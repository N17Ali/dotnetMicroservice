# Makefile for .NET Microservice K8S deployment

# Define image name and tag
COMMANDS_IMAGE = n17ali/commandsservice:latest
PLATFORMS_IMAGE = n17ali/platformsservice:latest

# Define K8S manifest files
COMMANDS_DEPL = K8S/commands-depl.yaml
PLATFORMS_DEPL = K8S/platforms-depl.yaml
PLATFORMS_CONFIG = K8S/platforms-service-config.yaml

# Target to set Minikube Docker environment
minikube-env:
	@echo "Setting Docker environment to Minikube..."
	eval $(minikube docker-env)

# Target to reset Docker environment
reset-env:
	@echo "Resetting Docker environment..."
	eval $(minikube docker-env -u)

# Target to build Commands Service Docker image
# Depends on minikube-env to ensure Docker builds inside minikube
build-commands: minikube-env
	@echo "Building Commands Service Docker image $(COMMANDS_IMAGE)..."
	docker build -t $(COMMANDS_IMAGE) ./CommandsService
	@echo "Commands Service image built."
	$(MAKE) reset-env # Reset env after building

# Target to build Platforms Service Docker image
# Depends on minikube-env to ensure Docker builds inside minikube
build-platforms: minikube-env
	@echo "Building Platforms Service Docker image $(PLATFORMS_IMAGE)..."
	docker build -t $(PLATFORMS_IMAGE) ./PlatformsService
	@echo "Platforms Service image built."
	$(MAKE) reset-env # Reset env after building

# Target to apply Commands Service K8S manifests
apply-commands:
	@echo "Applying Commands Service K8S manifests ($(COMMANDS_DEPL))..."
	kubectl apply -f $(COMMANDS_DEPL)
	@echo "Commands Service K8S manifests applied."

# Target to apply Platforms Service K8S manifests (assuming they exist)
apply-platforms:
	@echo "Applying Platforms Service K8S manifests ($(PLATFORMS_DEPL)) and config ($(PLATFORMS_CONFIG))..."
	kubectl apply -f $(PLATFORMS_DEPL)
	kubectl rollout restart deployment # Restart deployment to apply changes
	kubectl apply -f $(PLATFORMS_CONFIG)
	@echo "Platforms Service K8S manifests applied."

# Default target - build and apply commands service
all: build-platforms build-commands apply-platforms apply-commands

pods:
	kubectl get pods

deployments:
	kubectl get deployments

services:
	kubectl get services

.PHONY: all build-commands apply-commands minikube-env reset-env build-platforms apply-platforms pods deployments services

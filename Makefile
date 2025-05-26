# Makefile for .NET Microservice K8S deployment

# Define image name and tag
COMMANDS_IMAGE = n17ali/commandsservice:latest
PLATFORMS_IMAGE = n17ali/platformsservice:latest

# Define K8S manifest files
COMMANDS_DEPL = K8S/commands-depl.yaml
PLATFORMS_DEPL = K8S/platforms-depl.yaml
PLATFORMS_CONFIG = K8S/platforms-service-config.yaml
PLATFORMS_NP_SRV = K8S/platforms-np-srv.yaml

# Target to set Minikube Docker environment (no longer a dependency for build targets)
minikube-env:
	@echo "Setting Docker environment to Minikube..."
	eval $(minikube docker-env)

# Target to reset Docker environment 
reset-env:
	@echo "Resetting Docker environment..."
	eval $(minikube docker-env -u)

# Target to build Commands Service Docker image
build-commands:
	@echo "Building Commands Service Docker image $(COMMANDS_IMAGE) inside Minikube..."
	minikube image build -t $(COMMANDS_IMAGE) ./CommandsService
	@echo "Commands Service image built."

# Target to build Platforms Service Docker image
build-platforms:
	@echo "Building Platforms Service Docker image $(PLATFORMS_IMAGE) inside Minikube..."
	minikube image build -t $(PLATFORMS_IMAGE) ./PlatformsService
	@echo "Platforms Service image built."

# Target to apply Commands Service K8S manifests
apply-commands:
	@echo "Applying Commands Service K8S manifests ($(COMMANDS_DEPL))..."
	kubectl apply -f $(COMMANDS_DEPL)
	kubectl rollout restart deployment commands-depl
	@echo "Commands Service K8S manifests applied."

# Target to apply Platforms Service K8S manifests (assuming they exist)
apply-platforms:
	@echo "Applying Platforms Service K8S manifests ($(PLATFORMS_DEPL)) and config ($(PLATFORMS_CONFIG))..."
	kubectl apply -f $(PLATFORMS_DEPL)
	kubectl rollout restart deployment platforms-depl 
	kubectl apply -f $(PLATFORMS_CONFIG)
	kubectl apply -f $(PLATFORMS_NP_SRV)
	@echo "Platforms Service K8S manifests applied."

# Default target - build and apply commands service
all: build-platforms build-commands apply-platforms apply-commands

pods:
	kubectl get pods

deployments:
	kubectl get deployments

services:
	kubectl get services

logs:
	@if [ -z "$(POD_NAME)" ]; then \
		echo "Error: POD_NAME is not set. Usage: make logs POD_NAME=<your-pod-name>"; \
		exit 1; \
	fi
	@echo "Streaming logs for pod: $(POD_NAME)..."
	kubectl logs -f $(POD_NAME)

.PHONY: all build-commands apply-commands minikube-env reset-env build-platforms apply-platforms pods deployments services logs
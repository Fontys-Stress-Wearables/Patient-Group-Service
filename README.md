# Patient Group Service

## Introduction
The Patient Group Service is in charge of managing patient groups (adding, updating and removing).
It is also in charge of adding caregivers and patients to patient groups.


## Build steps

### development
To run the project locally you can run `docker compose up`.
This will build the API and run all the services necessary for it to function properly.
If there is no NATS service running you can start it by first running  `docker compose -f docker-compose-nats.yaml up -d`.

## Docker
To get the project running in kubernetes there are a couple of steps:
- build the image for the backend by running `docker build -t ghcr.io/fontys-stress-wearables/patient-group-service:main .`
- Push the image to the docker registry by running `docker push ghcr.io/fontys-stress-wearables/patient-group-service:main`

# Patient Group Service

## Introduction
The Patient Group Service is in charge of managing patient groups (adding, updating and removing).
It is also in charge of adding caregivers and patients to patient groups.


## Build steps

### development
To build and run the project locally you can run `docker compose up`
This will build the API and run all the services necessary for it to function properly.
If there is no NATS service running you can start it along with the other services by running  `docker compose -f docker-compose-nats.yaml up`.

## Production
To get the project running in kubernetes there are a couple of steps:
- build the image for the backend by running `docker build -f Patient-Group-Service/Dockerfile -t <HOST>/patient-group-service:<TAG>`.
- Push the image to the docker registry by running `docker push <HOST>/patient-group-service:<TAG>`
- add the service and the database to kubernetes by running `kubectl apply -f api.yaml -f db.yaml`. 
If there is no NATS service running you can deploy it along with the other services by running `kubectl apply -f api.yaml -f db.yaml -f nats.yaml`
-

Alternatively you can execute these steps by running the `deploy.sh` script. You can set the host and tag by passing the "h" and "t" flags. For example `./deploy.sh -t latest -h localhost:5001`.
If you just want to update the kubernetes configuration you can run `./deploy.sh -s`. 
The `-s` option skips the docker build steps.
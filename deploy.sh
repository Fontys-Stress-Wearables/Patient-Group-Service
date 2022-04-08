#!/bin/bash

tag="latest"
host="localhost:5001"
image_name="patient-group-service"
docker_dir="Patient-Group-Service"
docker_context="."
skip_rebuild=false

while getopts t:h:s flag
do
    case "${flag}" in
        t) tag=${OPTARG};;
        h) host=${OPTARG};;
        s) skip_rebuild=true;;
    esac
done

if [ "$skip_rebuild" = false ]; then
    # build backend and push the image to the registry
    docker build -f $docker_dir/Dockerfile -t $host/$image_name:$tag $docker_context
    docker push $host/$image_name:$tag
fi

cd kubernetes

# check if the nats service has already been added, if it hasn't add it
kubectl diff -f nats.yml &>/dev/null
rc=$?
if [ $rc -eq 0 ];then
    kubectl apply -f api.yaml -f db.yaml 
else
    kubectl apply -f api.yaml -f db.yaml -f nats.yaml 
fi



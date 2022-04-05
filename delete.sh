#!/bin/bash

cd kubernetes

# check if the nats service is present, if it is also delete it
kubectl diff -f nats.yml &>/dev/null
rc=$?
if [ $rc -eq 0 ];then
    kubectl delete -f db.yaml -f api.yaml 
else
    kubectl delete -f db.yaml -f api.yaml -f nats.yaml
fi

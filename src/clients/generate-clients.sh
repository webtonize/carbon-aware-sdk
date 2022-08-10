#!/bin/bash
if [ -z "$1" ]
then
    echo "You must provide the first parameter as the host name of the service to generate the clients for (ex. localhost:5073)."
    exit 1
fi

rm -r ./java
openapi-generator-cli generate -i http://$1/swagger/v1/swagger.json -g java -o ./java
rm -r ./python
openapi-generator-cli generate -i http://$1/swagger/v1/swagger.json -g python -o ./python
rm -r ./javascript
openapi-generator-cli generate -i http://$1/swagger/v1/swagger.json -g javascript -o ./javascript
rm -r ./csharp
openapi-generator-cli generate -i http://$1/swagger/v1/swagger.json -g csharp-netcore -o ./csharp --additional-properties=targetFramework=net6.0
rm -r ./golang
openapi-generator-cli generate -i http://$1/swagger/v1/swagger.json -g go -o ./golang

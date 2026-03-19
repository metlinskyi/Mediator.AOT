Implement a universal POST endpoint with the next pattern "mediator/send/{className}"
where className is name of type of any class/record implemented IRequest.
Call the Handle method of matched implementation handler IRequestHandler.
Run the application in docker container.  
The implementation have to support Native AOT.

Example:
    request: 
                POST mediator/send/HelloRequest
                {

                }
    response: 
                { 
                    Message: "Hello World!"
                } 


docker build -t aot-api .
docker run -p 8080:8080 aot-api






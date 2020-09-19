# StreetOfThings
## Summary:
StreetOfThings enables its users to share, rent or gift items they do not need with / to their neighbors through the application, As most homes have things that they need at certain limited times, and most of the time they are left unused to wear out over time.
The importance of implementing the Street of Things app stems from the fact that it strengthens social relations at the level of the people of the neighborhood and saves the residents of the neighborhood who do not have the price of the item they need the ability to borrow it from their neighbors or perhaps obtain it for free, Also, the owner of the object can achieve profit by offering the object for rent for a specified period, thus making use of the shelf life of things as much as possible.
In the end, we can only say that mobile applications have become one of the most important technologies that contribute to facilitating people's lives, which is changing the nature of our relationship with the things we own so we do not cling to things much, and also change our relationship with the people around us for the better.

This repository is only the back-end for StreetOfThings.

## Microservices
microservices architecture is an approach to building a server application as a set of small services. That means a microservices architecture is mainly oriented to the back-end, Each service runs in its own process and communicates with other processes using protocols such as HTTP/HTTPS, WebSockets, or AMQP. Each microservice implements a specific end-to-end domain or business capability within a certain context boundary, and each must be developed autonomously and be deployable independently. Finally, each microservice should own its related domain data model and domain logic (sovereignty and decentralized data management) and could be based on different data storage technologies (SQL, NoSQL) and different programming languages. 

### why build microservice
* Independent components.
* Easier understanding.
* Better scalability.
* Flexibility in choosing the technology.
* The higher level of agility: Any fault in a microservices application affects only a particular service and not the whole solution

### Weaknesses of the Microservice Architecture:
* Extra complexity.
* System distribution. 
* Cross-cutting concerns.
* Testing.
* Microservices Are Often More Expensive Than Monoliths: 

### Why did we choose the microservices architecture:
although all those disadvantages listed above, plus our application did not require the urge to scale so vigorously we could have just built our back-end as a huge monolith server and upgrade it as our application grows, but we choose to architect the back-end as microservices just for the sake of learning it, architect small parts of software to solve huge problems.
The .NET core was the best candidate for embracing a microservices-oriented system that is based on containers because .NET Core is lightweight. In addition, its related container images, either the Linux image or the Windows Nano image, are lean and small making containers light and fast to start. 

## Containerizing the back-end.
Containerization is an approach to software development in which an application or service, its dependencies, and its configuration (abstracted as deployment manifest files) are packaged together as a container image. The containerized application can be tested as a unit and deployed as a container image instance to the host operating system (OS). All of our microservices and there databases are containerized with [docker](https://www.docker.com).

[Authorization service DockerFile](https://github.com/Abdulrhman5/StreetOfThings/blob/master/Services/Authorization/AuthorizationService/Dockerfile)

## Microservices of StreetOfThings
![services image ](https://github.com/Abdulrhman5/StreetOfThings/blob/master/services.jpg?raw=true)

## Gateways
### What is the API Gateway pattern? 
When designing and building large or complex microservice-based applications with multiple client apps, a good approach to consider can be an API Gateway. This is a service that provides a single entry point for certain groups of microservices. It’s similar to the Facade pattern from object-oriented design, but in this case, it’s part of a distributed system. The API Gateway pattern is also sometimes known as the “backend for frontend” (BFF) because you build it while thinking about the needs of the client app. 
Therefore, the API gateway sits between the client apps and the microservices. It acts as a reverse proxy, routing requests from clients to services. also provide additional cross-cutting features such as authentication, and cache.

### Main features in the API Gateway pattern
* Reverse proxy or gateway routing. The MobileGateway, AdminGateway offers a reverse proxy to redirect or route requests (layer 7 routings, usually HTTP requests) to the endpoints of the internal microservices.
* Requests aggregation. As part of the gateway pattern, you can aggregate multiple client requests (usually HTTP requests) targeting multiple internal microservices into a single client request. 

## what have we used for communications between services:

*. [gRPC](https://grpc.io)

*. [RabbitMQ](https://www.rabbitmq.com/)

*. REST.

## Databases: 
Databases should be treated as private to each microservice. No other microservice can directly modify data stored inside the database in another microservice. and accessible only via its API, And for that in the back-end, we have three independent databases.
Authorization database, Catalog database, Transaction database.
there is a container named SQL.data defined in the docker-compose.yml file that runs SQL Server for Linux with all the SQL Server databases needed for the microservices. (You could also have one SQL Server container for each database, but that would require more memory assigned to Docker.) The important point in microservices is that each microservice owns its related data, therefore its related SQL database in this case. But the databases can be anywhere. We also are using the Local SQL server Database for development.

### Accessing the database from code:
For that we used [Entity framework core](https://docs.microsoft.com/en-us/ef/core/)

## Internal design for each microservice: 
In our StreetOfThings app, We used something similar to [Domain Oriented N-Layered Architecture V2.0 Published first ALPHA version of Domain Oriented N-Layered Architecture V2.0 | Cesar de la Torre](https://devblogs.microsoft.com/cesardelatorre/published-first-alpha-version-of-domain-oriented-n-layered-architecture-v2-0/) as an internal design for our services


# Note
[The source of this readme](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/)

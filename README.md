# StreetOfThings
## Summary:
From here we went to create the Street of Things app Which enables its users to share, rent or gift items they do not need with / to their neighbors through the application, As most homes have things that they need at certain limited times, and most of the time they are left unused to wear out over time.
The importance of implementing the Street of Things app stems from the fact that it strengthens social relations at the level of the people of the neighborhood and saves the residents of the neighborhood who do not have the price of the item they need the ability to borrow it from their neighbors or perhaps obtain it for free, Also, the owner of the object can achieve profit by offering the object for rent for a specified period, thus making use of the shelf life of things as much as possible.
In the end, we can only say that mobile applications have become one of the most important technologies that contribute to facilitating people's lives, which is changing the nature of our relationship with the things we own so we do not cling to things much, and also change our relationship with the people around us for the better.
[Street bank](https://www.streetbank.com/splash?locale=en)
# Back-end
A set of microservices built with ASP.net

## Microservices
microservices architecture is an approach to building a server application as a set of small services. That means a microservices architecture is mainly oriented to the back-end, although the approach is also being used for the front end. Each service runs in its own process and communicates with other processes using protocols such as HTTP/HTTPS, WebSockets, or AMQP. Each microservice implements a specific end-to-end domain or business capability within a certain context boundary, and each must be developed autonomously and be deployable independently. Finally, each microservice should own its related domain data model and domain logic (sovereignty and decentralized data management) and could be based on different data storage technologies (SQL, NoSQL) and different programming languages. 

### why build microservice
* Idependent components.
* Easier understanding.
* Better scalability.
* Flexibility in choosing the technology.
* The higher level of agility: Any fault in a microservices application affects only a particular service and not the whole solution

### Why did we choose the microservices architecture:
although all those disadvantages listed above, plus our application did not require the urge to scale so vigorously we could have just built our back-end as a huge monolith server and upgrade it as our application grows, but we choose to architect the back-end as microservices just for the sake of learning it, architect small parts of the software to solve huge problems.
The .NET core was the best candidate for embracing a microservices-oriented system that is based on containers because .NET Core is lightweight. In addition, its related container images, either the Linux image or the Windows Nano image, are lean and small making containers light and fast to start. 


## Containerizing the back-end.
Containerization is an approach to software development in which an application or service, its dependencies, and its configuration (abstracted as deployment manifest files) are packaged together as a container image. The containerized application can be tested as a unit and deployed as a container image instance to the host operating system (OS).

## What is Docker?
Docker is an open-source project for automating the deployment of applications as portable, self-sufficient containers that can run on the cloud or on-premises. Docker is also a company that promotes and evolves this technology, working in collaboration with cloud, Linux, and Windows vendors, including Microsoft
[Authorization service DockerFile](https://github.com/Abdulrhman5/StreetOfThings/blob/master/Services/Authorization/AuthorizationService/Dockerfile)


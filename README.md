webApiAngular
=============

Detailed sample shows how webapi2/angular/NEventStore/Ninject work together

## Why I build this?
I was invited by a startup company, and was asked to finish a test to verify my skills. The problem is simple, and can be done via transcript pattern. But I want to do it seriously and put all good stuffs I am good at to have a little bit fun. It took me the whole weekend (around 20 hours) to write all the source code from scratch. 

## What is the problem to solve?
To write a proof-of-concept to allow employees of a company to apply for leave through a reactive web application. The app will let employees request leave with a start and end date; a reason (Annual, Personal, Compassionate or Parental); and comments, and then allow then to submit the request to their manager for approval. It will also allow the employee to see previous requests they have made.

## What is the architecture?
CQRS + Event Store + WebApi + Angular

## What is the detailed tech stacks?
* **Bearer token** by using Asp.net owin identity: I always think it is the pure way to do with angular, and another benefits is the whole api can serve mobile api without any changes;
* **Ioc** by using Ninject
* **CQRS**: totally separated the write and read side, be able to use other database for read-side like mongo, elastic search;  
* **Event store** by using NEventStore: I like event sourcing concept, which means like the source of truth is the transaction logs instead of final states, and event listener can help put into suitable databases for read.
* **Message** bus by a fake in-memory bus: I think it can be replaced by real bus easily like NServiceBus; 
* **Dapper** for read model
* Client side: 
  * **ui.router**: replace built-in angular-router;
  * **restangular**: replace built-in angular-resource;
  * **coffeescript**: I like coffeescript, because the code will be concise, more readable, less errors, and more object oriented and more functional programming supported;
  * **scss**  

## How to setup this solution?
* Create a SQL Server database, and put the connection string in web.config (3 places: for identity, event store, and read store);
* I use the bower to manger the client-side library, and also use grunt to  build the client side. So the detailed steps are:
  * install node.js;
  * npm install bower -g
  * npm install grunt -g
  * cd assets folder, run: 
    * npm install
    * bower install
    * grunt
* Build the c# code, and visit http://localhost:{port}/assets/index.html. 

## Source code structure
* webApiAngular.API
  * App_Start: route settings, centrelised api exception, app config, etc
  * Controller: 
  * Domain: domain modals, event handlers, command handlers, queries
  * Infrastructure: FakeBus, FakeBus dispacher(used for event store), abstract aggregate class, event streem repository, read model repository, 
  * assets: all the angular app, scss, etc

## Requirements comments:
* Main features:
  * Signin / Signup 
  * Two roles:
    * User: 
    * Manger: only one manager globally, to be a manager just sign up with the same email address in web.config->appSettings->manager, and sign in
  * User can do:
    * Apply leave
    * List his own leaves
    * Show statistics in dashboard
    * Auto calculate working days, it will remove weekend and public holidays
  * Manager can do:
    * List all pending leaves;
    * Evaluate certain leave: approve or reject;
  * Validation:
    * All client-side validations I think of
    * Main server-side validations like:  
      * Cannot overlap with the existing approved leaves
      * Cannot apply for 0 working days, like apply 25/12 and 26/12
* I didn't do the following due to short time:
  * Email verification procedure: currently, when sign up, it will be activated immediately;
  * Reset or change password;
  * Hide the Evaluate menu when it was not a manager, currently it still shows but it will give you 401 if you click into;

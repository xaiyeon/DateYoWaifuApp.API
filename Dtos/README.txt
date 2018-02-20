Dtos stands for Data Transfer Object, which is for holding data of an object.
So instead of passing a direct object of our exact class with specific parameters, we can instead pass an object
and define it from there. This makes it much easier to use and program.

It also makes sense to add Data Annotations here for our data going to our API and adding to the database


In the lesson Section 8. We used AutoMapper to shape and link our Dto and data.

DTO are Data-Transfer-Objects that do not contain any business logic, but provide simple setters and getters for
accessing data from other models. They are like a middle layer between actual models and the controller/business logic.
This way you do not ruin your database object models.
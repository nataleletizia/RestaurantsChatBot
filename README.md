# ChatBot
Chat Bot for restaurants booking, utilizes Microsoft Bot Framework + API of Language Understanding Intelligent Service (Cognitive Support) (LUIS)

Planning and Architecture choices:

Planned choice was to utilize state of art Artificial Intelligence Software Architectures in .NET as backend of the dialog system 

The choice relied on ASP.NET MVC, to structure a software mediator/chat bot giving greeting messages and serving the end user by providing the most
appropriate candidate restaurants based on her/his preferences. The software utilizes asynchronous calls to request by remote message endpoints services 
cognitive understanding services provided from the learning machine (LUIS) service hosted on the cloud. The set of utterances/entities/intents filtered from
the cognitive service, are then delivered at the software which gathers the customer requirements, and from the provision of the customer departing point
and the area of research of the restaurants, is capable to provide answers according to such criteria formed, accordingly if matching restaurants are in the
area researched from the customers.

A better integration between the existing services provided for table booking from Google and restaurants owners, could lead to further enhancement to the
software.

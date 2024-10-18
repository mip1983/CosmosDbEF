Two projects with same code demonstrating differences/breakages going from EF8 to EF9 with ComosDb. 
Set a console app as startup project, configure your cosmosDb connection in user secrets and run for each console app in turn.

You will see the errors when running the EF9 version when you start. 
There is code in 'TestDbContext' you can comment out for some current work arounds, but I don't know how to deal with the nullable DatetimeRange correctly.

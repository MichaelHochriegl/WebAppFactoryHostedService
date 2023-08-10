# WebAppFactoryHostedService
Example of the strange behavior of the `WebApplicationFactory` with top level statements and e.g. `HostedService`

You can see the IMHO wrong behavior if you start the test that is in here.
The app runs before the `MovieWebApplicationFactory` can switch out the connection string for the `MovieDbContext`, the test fails due to the `MovieHostedService` already running with the production connection string.

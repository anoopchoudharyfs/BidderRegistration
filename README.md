##Bidder Registration

Bidder registrations job is to provide an API which enables the user to update a users bidding status on an auction.

##MVP

The MVP provides one endpoint, allowing you to update a bidders status on an auction which will be stored in a CosmosDB collection.

![Artifacts](./docs/BidderRegWidgetAct.PNG)

##Developer Notes

**Solution Structure**

The solution has a simple structure with only three projects contained inside it.

**API**: Contains the endpoints, config and startup for the project. It references the other two projects.

**Domain**: Contains the internal representations of the business objects and logic. It is referenced from the other two projects.

**Infrastructure**: Contains the code which calls external infrastructure – currently this consists entirely of CosmosDB. It references the domain project.

**Cosmos Database**

The BidderRegistration database uses a CosmosDB with eventual consistency to store bidder status data.

The partition key is:

bidderRequest.CustomerId-bidderRequest.MarketplaceCode.

The ID is:

bidderRequest.AuctionId-bidderRequest.CustomerId-bidderRequest.MarketplaceCode.

An example document from the container can be seen below:
```json
{
    "id": "1-a_customer_123-201",
    "partitionKey": "a_customer_123-201",
    "marketplaceCode": 201,
    "globalAuctionId": 1,
    "auctionHouseId": 1001,
    "customerId": "a_customer_123",
    "bidderId": "a-bidder-123",
    "paddleNumber": "10C",
    "status": 3,
    "cta": "The beach was crowded with snow leopards.\\nThere was no ice cream in the freezer, nor did they have money to go to the store.\\nGreen should have smelled more tranquil, but somehow it just tasted rotten.",
    "_rid": "fQ5+ANYr1b4BAAAAAAAAAA==",
    "_self": "dbs/fQ5+AA==/colls/fQ5+ANYr1b4=/docs/fQ5+ANYr1b4BAAAAAAAAAA==/",
    "_etag": "\"88007074-0000-0100-0000-6238acf30000\"",
    "_attachments": "attachments/",
    "_ts": 1647881459
}
```
**Restoring Data**

CosmosDB data can be restored using the point and time restore process:
https://docs.microsoft.com/en-us/azure/cosmos-db/restore-account-continuous-backup

**External Dependencies**

The bidder registration service is dependent on the CosmosDB in order to function.


##Platform Integration Notes

**Client Timeouts**

The recommended timeout for the Bidder Registration endpoints is 2 seconds.

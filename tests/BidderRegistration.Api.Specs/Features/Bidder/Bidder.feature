Feature: Create or Update Bidders Approval Status
	As a Platform User
	I want to create or Update a bidder's Status (i.e Approved, Pending, Denied) record in a bidder status datastore
	So that that bidder's status can be verified ahead of or at the time of placing bids by checking against the bidder's record stored in the datastore

Background:
	Given my request body is
		"""
		{
			"MarketplaceCode": 201,
			"AuctionId" : 123,
			"AuctionHouseId" : 20,
			"customerId" : "a_Customer_id",
			"bidderId" :"a_Bidder_id",
			"Status": "Approved", 
			"bidderRef": "10C",  
			"cta" : "test html"
		}
		"""	
	And my required headers
		| Key                   | Value |
		| x-bid-source-platform | 10    |
		| x-bid-client-id       | 1     |
		| x-bid-client-ip       | 1     |
		| x-bid-app-id          | 1     |
		| x-bid-user-reference  | 1     |

Scenario:Request with valid details
	When you send a put bidder request to /v1/bidder
	Then response should be 200 OK
	And the bidder should be persisted in the database as
	| PartitionKey      | Id                    | MarketplaceCode | AuctionId | AuctionHouseId | customerId    | bidderId    | Status   | bidderRef | cta       |
	| a_Customer_id-201 | 123-a_Customer_id-201 | 201                | 123          | 20                | a_Customer_id | a_Bidder_id | Approved | 10C       | test html |


Scenario:Request with Invalid details
	 Given my request body is
		"""
		{
			"MarketplaceCode": 201,
			"AuctionId" : "invalidauctionid",
			"AuctionHouseId" : 20,
			"customerId" : "a_Customer_id",
			"bidderId" :"a_Bidder_id",
			"Status": "Approved", 
			"bidderRef": "10C",  
			"cta" : "test html"
		}
		"""	
	When you send a put bidder request to /v1/bidder
	Then response should be 400 Bad Request

	
Scenario:Request with Invalid Status
	 Given my request body is
		"""
		{
			"MarketplaceCode": 201,
			"AuctionId" : 30,
			"AuctionHouseId" : 20,
			"customerId" : "a_Customer_id",
			"bidderId" :"a_Bidder_id",
			"Status": "Invalid_Status", 
			"bidderRef": "10C",  
			"cta" : "test html"
		}
		"""	
	When you send a put bidder request to /v1/bidder
	Then response should be 400 Bad Request
	And the response contains only these validation errors
		| code | value              | path     | description                                                                                                                                                                 |
		| 100  | ERROR_MISSING_DATA | $.Status | The JSON value could not be converted to System.Nullable`1[BidderRegistration.Domain.Status]|

Scenario:Request when Status is None
	 Given my request body is
		"""
		{
			"MarketplaceCode": 201,
			"AuctionId" : 30,
			"AuctionHouseId" : 20,
			"customerId" : "a_Customer_id",
			"bidderId" :"a_Bidder_id",
			"Status": "None", 
			"bidderRef": "10C",  
			"cta" : "test html"
		}
		"""	
	When you send a put bidder request to /v1/bidder
	Then response should be 400 Bad Request
	And the response contains only these validation errors with error codes
		| code | value                | path   | description                         |
		| 1008 | ERROR_INVALID_STATUS | Status | The Status field should not be None |

Scenario:Request with Pending Status requires a cta
	 Given my request body is
		"""
		{
			"MarketplaceCode": 201,
			"AuctionId" : 30,
			"AuctionHouseId" : 20,
			"customerId" : "a_Customer_id",
			"bidderId" :"a_Bidder_id",
			"Status": "Pending", 
			"bidderRef": "10C",  
			"cta" : ""
		}
		"""	
	When you send a put bidder request to /v1/bidder
	Then response should be 400 Bad Request
	And the response contains only these validation errors
		| code | value              | path | description                                                          |
		| 100  | ERROR_MISSING_DATA | Cta  | The CTA field should not be empty when Status is Pending or Declined |

Scenario:Request with Denied Status requires a cta
	 Given my request body is
		"""
		{
			"MarketplaceCode": 201,
			"AuctionId" : 30,
			"AuctionHouseId" : 20,
			"customerId" : "a_Customer_id",
			"bidderId" :"a_Bidder_id",
			"Status": "Denied", 
			"bidderRef": "10C",  
			"cta" : " "
		}
		"""	
	When you send a put bidder request to /v1/bidder
	Then response should be 400 Bad Request
	And the response contains only these validation errors
		| code | value              | path | description                                                          |
		| 100  | ERROR_MISSING_DATA | Cta  | The CTA field should not be empty when Status is Pending or Declined |



Scenario: Empty string request
	Given my request body is
		"""
		"""
	When you send a put bidder request to /v1/bidder
	Then response should be 400 Bad Request


Scenario: Empty json object request
	Given my request body is
		"""
		{}
		"""
	When you send a put bidder request to /v1/bidder
	Then response should be 400 Bad Request
	And the response contains only these validation errors
		| code | value              | path               | description                               |
		| 100  | ERROR_MISSING_DATA | MarketplaceCode | The MarketplaceCode field is required. |
		| 100  | ERROR_MISSING_DATA | AuctionId       | The AuctionId field is required.       |
		| 100  | ERROR_MISSING_DATA | AuctionHouseId  | The AuctionHouseId field is required.  |
		| 100  | ERROR_MISSING_DATA | CustomerId         | The CustomerId field is required.         |
		| 100  | ERROR_MISSING_DATA | BidderId           | The BidderId field is required.           |
		| 100  | ERROR_MISSING_DATA | Status             | The Status field is required.             |
		| 100  | ERROR_MISSING_DATA | BidderRef          | The BidderRef field is required.          |
		
			
Scenario: Request without required headers
	And the correlation id is "1211"
	And the current time is 2021-11-04T11:25:43.263Z
	And my required headers
		| Key | Value |
		
	When you send a put bidder request to /v1/bidder
	Then response should be 400 Bad Request


Scenario: Request with marketplace code that doesnt exist fails.
Given my request body is
		"""
		{
			"MarketplaceCode": 9999,
			"AuctionId" : 123,
			"AuctionHouseId" : 20,
			"customerId" : "a_Customer_id",
			"bidderId" :"a_Bidder_id",
			"Status": "Approved", 
			"bidderRef": "10C",  
			"cta" : "test html"
		}
		"""	
	And my required headers
		| Key                   | Value |
		| x-bid-source-platform | 10    |
		| x-bid-client-id       | 1     |
		| x-bid-client-ip       | 1     |
		| x-bid-app-id          | 1     |
		| x-bid-user-reference  | 1     |

	When you send a put bidder request to /v1/bidder
	Then response should be 400 Bad Request
	And the response contains only these validation errors with error codes
		| code | value                              | path               | description                        |
		| 1010 | ERROR_NOTEXISTS_MARKETPLACECODE | MarketplaceCode | MarketplaceCode does not exist. |


Scenario: Request with invalid marketplace code fails.
Given my request body is
		"""
		{
			"MarketplaceCode": 0,
			"AuctionId" : 123,
			"AuctionHouseId" : 20,
			"customerId" : "a_Customer_id",
			"bidderId" :"a_Bidder_id",
			"Status": "Approved", 
			"bidderRef": "10C",  
			"cta" : "test html"
		}
		"""	
	And my required headers
		| Key                   | Value |
		| x-bid-source-platform | 10    |
		| x-bid-client-id       | 1     |
		| x-bid-client-ip       | 1     |
		| x-bid-app-id          | 1     |
		| x-bid-user-reference  | 1     |

	When you send a put bidder request to /v1/bidder
	Then response should be 400 Bad Request
	And the response contains only these validation errors with error codes
		| code | value                            | path               | description                                              |
		| 1006 | ERROR_INVALID_MARKETPLACECODE | MarketplaceCode | The MarketplaceCode field should be greater than zero |
		| 1010 | ERROR_NOTEXISTS_MARKETPLACECODE | MarketplaceCode | MarketplaceCode does not exist. |




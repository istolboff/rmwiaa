Feature: Top Level scenarios

Scenario: Notify when near the location
	Given I setup the following reminder
	| Location                             | Message                              |
	| 55.770408630371094,37.61994552612305 | 4F95A5D3 AEA5 4D81 A88C F4A3A9DD1922 |
	When I am near the 55.770408630371094,37.61994552612305 location
	Then I want to be notified with the message '4F95A5D3 AEA5 4D81 A88C F4A3A9DD1922'

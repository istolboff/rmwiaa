Feature: Top Level scenarios

	Scenario: Notify when near the location
		Given user A sets up the following reminder
			| Location                             | Message                              |
			| 55.770408630371094,37.61994552612305 | 4F95A5D3 AEA5 4D81 A88C F4A3A9DD1922 |
		When user A gets near the 55.770408630371094,37.61994552612305 location
		Then user A should be reminded with the message '4F95A5D3 AEA5 4D81 A88C F4A3A9DD1922'

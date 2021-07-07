This sample works in a following way:

1. Creates an entry in Table Storage
2. Creates a new subscription in Service Bus
3. Deploys a new Azure Function with ServiceBus Trigger. Function reacts to the 
previously registered subscription and has some dummy code in .csx file for sending HTTP requests.
   
Additional functionality:
1. Redeployment of all functions (needed in case of .csx file update)
2. Removal of selected azure functions and table storage row.
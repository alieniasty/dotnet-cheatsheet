import requests
import json
import sys

try:
    print(sys.argv)

    patAdoAuth = sys.argv[2]

    getHeaders = {
    'Authorization': patAdoAuth,
    }
    
    updatePrHeaders = {
    'Authorization': patAdoAuth,
    'Content-Type': 'application/json',
    }

    getPrUrl = "***/pullrequests?searchCriteria.targetRefName=refs/heads/master&api-version=5.0"
    getLastCommitOnMasterUrl = "***/commits?searchCriteria.$top=1&searchCriteria.itemVersion.version=master&api-version=5.0"

    #Get Pull request    
    getPrResponse = requests.request("GET", getPrUrl, headers=getHeaders)
    getPrResponseJson = json.loads(getPrResponse.content)

    pullRequestId = getPrResponseJson["value"][0]["pullRequestId"]
    creatorId = getPrResponseJson["value"][0]["createdBy"]["id"]

    #Get last commit on master    
    response = requests.request("GET", getLastCommitOnMasterUrl, headers=getHeaders)
    lastCommitOnMaster = getPrResponseJson["value"][0]["lastMergeSourceCommit"]

    #Update Pull request    
    updatePrUrl = f"***/pullrequests/{pullRequestId}?api-version=5.0"

    prTitle = sys.argv[1]

    updateTitlePayload = json.dumps({
    "title": prTitle,
    })

    updatePrPayload = json.dumps({
    "LastMergeSourceCommit" : lastCommitOnMaster,
    "autoCompleteSetBy": f"{creatorId}",
    "status" : "completed"
    })    

    requests.request("PATCH", updatePrUrl, headers=updatePrHeaders, data=updateTitlePayload)
    updatePrResponse = requests.request("PATCH", updatePrUrl, headers=updatePrHeaders, data=updatePrPayload)

    print(updatePrResponse)

    print("Completing Pull Request.")
except:
    print("Error completing Pull Request.")
    raise
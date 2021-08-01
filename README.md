# OzVaxProgress

Source code for [OzVaxProgress](https://twitter.com/oz_vax) twitter bot.

Uses vaccination and population data from https://github.com/owid/covid-19-data


# Running it yourself

## Building on GCP

`gcloud builds submit --tag gcr.io/[PROJECT-ID]/[image-name]`


## Secrets

Set the twitter secrets in GCP secret manager with the following names:

- TWITTER_CONSUMER_KEY
- TWITTER_CONSUMER_SECRET
- TWITTER_OZVAX_USER_ACCESS_TOKEN
- TWITTER_OZVAX_USER_ACCESS_TOKEN_SECRET

Use the following command either by passing a data file or echoing the secret and piping: 
`gcloud secrets create TWITTER_CONSUMER_KEY --data-file=`

See https://cloud.google.com/secret-manager/docs/creating-and-accessing-secrets?hl=en_GB for more details.


## Create service account

- Create a service account with the following roles:
  - Cloud datastore user: for firestore user access
  - Secret Manager Secret Accessor
  - Cloud run invoker 
- Add a key for this service account and use it for local development/testing.


## Deploy to cloud run

`gcloud beta run deploy [service-name] --service-account [service-account-email] --image gcr.io/[PROJECT-ID]/[image-name] --update-env-vars "GCP_PROJECT_ID=[your GCP project id]" --update-secrets "TWITTER_CONSUMER_KEY=TWITTER_CONSUMER_KEY:latest,TWITTER_CONSUMER_SECRET=TWITTER_CONSUMER_SECRET:latest,TWITTER_OZVAX_USER_ACCESS_TOKEN=TWITTER_OZVAX_USER_ACCESS_TOKEN:latest,TWITTER_OZVAX_USER_ACCESS_TOKEN_SECRET=TWITTER_OZVAX_USER_ACCESS_TOKEN_SECRET:latest"`

First run:
Choose region (if you havent set a default already), disallow unauthenticated invocations.

## Create firestore DB

Create native firestore DB in GCP console.


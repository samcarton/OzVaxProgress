# OzVaxProgress

Source code for [OzVaxProgress](https://twitter.com/oz_vax) twitter bot.

Uses vaccination and population data from https://github.com/owid/covid-19-data


# Bot Overview

- Aims to utilise Google cloud within free-tier limits.
- Dotnet aspnet core container built on GCP.
- Google cloud run app triggered by a private HTTP endpoint.
- Google cloud scheduler setup to trigger the cloud run app a couple times per day.
- Using google firestore document storage.

# Running it yourself

## Building on GCP

`gcloud builds submit --tag gcr.io/[PROJECT-ID]/[image-name]`


## Twitter setup

### Set up twitter developer account

- Sign up for a twitter developer account.
- Create a "project" and "app" in the twitter developer console.
- Generate/take note of the "Consumer Keys" - Key and Secret values. These will be used for `TWITTER_CONSUMER_KEY` and `TWITTER_CONSUMER_SECRET` env variables in the app. See __Secrets__ section.
- Enable "3 legged OAuth" for your app. Callback and website URLs are not important.

### Set up twitter bot account and credentials

- Create a new twitter account for your bot.
- Use the `OzVaxProgress.TwitterAuthApp` console app and pass your developer account consumer keys. (e.g. `dotnet run -k "your key" -s "your secret"`)
  - This will write out an URL you must visit with your bot twitter account.
  - Authorize the app, and take note of the PIN code.
  - Enter the PIN code into the console app.
  - If successful, it should write out your bot user account credentials. 
  - Take note of these to use in the `TWITTER_OZVAX_USER_ACCESS_TOKEN` and `TWITTER_OZVAX_USER_ACCESS_TOKEN_SECRET` environment variables.


## Secrets

Set the twitter secrets in GCP secret manager with the following names, using the values you took note of from the previous section:

- `TWITTER_CONSUMER_KEY`
- `TWITTER_CONSUMER_SECRET`
- `TWITTER_OZVAX_USER_ACCESS_TOKEN`
- `TWITTER_OZVAX_USER_ACCESS_TOKEN_SECRET`

Use the following command either by passing a data file or echoing the secret and piping: 
`gcloud secrets create TWITTER_CONSUMER_KEY --data-file=`

See https://cloud.google.com/secret-manager/docs/creating-and-accessing-secrets?hl=en_GB for more details.


## Create service account

- Create a service account with the following roles:
  - Cloud datastore user: for firestore user access
  - Secret Manager Secret Accessor
  - Cloud run invoker 
- Add a key for this service account to use it for local development/testing.
  - Set the env variable `GOOGLE_APPLICATION_CREDENTIALS` to the location of your key file and run the aspnet app.

## Deploy to cloud run

`gcloud beta run deploy [service-name] --service-account [service-account-email] --image gcr.io/[PROJECT-ID]/[image-name] --update-env-vars "GCP_PROJECT_ID=[your GCP project id]" --update-secrets "TWITTER_CONSUMER_KEY=TWITTER_CONSUMER_KEY:latest,TWITTER_CONSUMER_SECRET=TWITTER_CONSUMER_SECRET:latest,TWITTER_OZVAX_USER_ACCESS_TOKEN=TWITTER_OZVAX_USER_ACCESS_TOKEN:latest,TWITTER_OZVAX_USER_ACCESS_TOKEN_SECRET=TWITTER_OZVAX_USER_ACCESS_TOKEN_SECRET:latest"`

First run:
Choose region (if you havent set a default already), disallow unauthenticated invocations.

## Create firestore DB

Create native firestore DB in GCP console.


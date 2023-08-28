# Foodremedy Terraform GCP

## Initial Setup
Follow Terraform GCP setup guide: https://developer.hashicorp.com/terraform/tutorials/gcp-get-started/google-cloud-platform-build

Repeating these steps should not be necessary unless a full rebuild of the project is required.

The `.tfstate` file for this project will be hosted in GCP in a storage bucket. This bucket was created manually and is not managed by Terraform.

In `main.tf` you can see the bucket configuration. 

```terraform
backend "gcs" {
    bucket = "foodremedy-api-tfstate"
    prefix = "env/dev"
}
```

This ensures resources managed by Terraform have a consistent state regardless of who is making changes.

## Terraform development

* Must have the Terraform CLI installed.
* Must have a credentials file for the service account in the `/terraform` directory named `credentials.json` (**DO NOT COMMIT TO GIT**)
* Run `terraform init` to connect to the GCP backend tfstate
* Make your changes, run `terraform plan` and set your env variable in the command line based on the environment you're developing for
* If the plan is as expected, run `terraform apply` to deploy your changes


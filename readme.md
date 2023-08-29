# FoodRemedy API

This API serves nutritional information for different foods through HTTP endpoints. 

Gopher Industries are building this product for an external client "FoodRemedy".

**This readme is a work in progress**

## API
### Authentication Endpoints

#### Login

**Endpoint**: `/auth/login`

**Method**: `POST`

**Tags**: Authentication

**Request Payload**:
```json
{
  "email": "string",
  "password": "string"
}
```

**Responses**:
- **200**: Success
  ```json
  {
    "tokenType": "string",
    "accessToken": "string",
    "expiresIn": 0,
    "refreshToken": "string"
  }
  ```
- **500**: Server Error

#### Refresh Access Token

**Endpoint**: `/auth/refresh`

**Method**: `POST`

**Tags**: Authentication

**Request Payload**:
```json
{
  "refreshToken": "string"
}
```

**Responses**:
- **200**: Success
  ```json
  {
    "tokenType": "string",
    "accessToken": "string",
    "expiresIn": 0,
    "refreshToken": "string"
  }
  ```
- **401**: Unauthorized
  ```json
  {
    "type": "string",
    "title": "string",
    "status": 0,
    "detail": "string",
    "instance": "string"
  }
- **500**: Server Error

### Ingredients Endpoints

#### List Ingredients

**Endpoint**: `/ingredients`

**Method**: `GET`

**Tags**: Ingredients

**Parameters**:
- `Skip` (integer, int32)
- `Take` (integer, int32)

**Responses**:
- **200**: Success
  ```json
  {
    "total": 0,
    "count": 0,
    "results": [
      {
        "id": "uuid",
        "description": "string"
      }
    ]
  }
  ```
- **401**: Unauthorized
  ```json
  {
    "type": "string",
    "title": "string",
    "status": 0,
    "detail": "string",
    "instance": "string"
  }
- **500**: Server Error

#### Create Ingredient

**Endpoint**: `/ingredients`

**Method**: `POST`

**Tags**: Ingredients

**Request Payload**:
```json
{
  "description": "string"
}
```

**Responses**:
- **201**: Created
  ```json
  {
    "id": "uuid",
    "description": "string"
  }
  ```
- **400**: Bad Request
  ```json
  {
    "type": "string",
    "title": "string",
    "status": 0,
    "detail": "string",
    "instance": "string"
  }
- **401**: Unauthorized
  ```json
  {
    "type": "string",
    "title": "string",
    "status": 0,
    "detail": "string",
    "instance": "string"
  }
- **500**: Server Error

#### Get Ingredient by ID

**Endpoint**: `/ingredients/{ingredientId}`

**Method**: `GET`

**Tags**: Ingredients

**Path Parameter**:
- `ingredientId` (string, uuid)

**Responses**:
- **200**: Success
  ```json
  {
    "id": "uuid",
    "description": "string"
  }
  ```
- **401**: Unauthorized
  ```json
  {
    "type": "string",
    "title": "string",
    "status": 0,
    "detail": "string",
    "instance": "string"
  }
- **404**: Not Found
  ```json
  {
    "type": "string",
    "title": "string",
    "status": 0,
    "detail": "string",
    "instance": "string"
  }
- **500**: Server Error

### Tags Endpoints

#### List Tags

**Endpoint**: `/tags`

**Method**: `GET`

**Tags**: Tags

**Parameters**:
- `Skip` (integer, int32)
- `Take` (integer, int32)

**Responses**:
- **200**: Success
  ```json
  {
    "total": 0,
    "count": 0,
    "results": [
      {
        "id": "uuid",
        "description": "string",
        "tagType": "string"
      }
    ]
  }
  ```
- **401**: Unauthorized
  ```json
  {
    "type": "string",
    "title": "string",
    "status": 0,
    "detail": "string",
    "instance": "string"
  }
- **500**: Server Error

#### Create Tag

**Endpoint**: `/tags`

**Method**: `POST`

**Tags**: Tags

**Request Payload**:
```json
{
  "description": "string",
  "tagType": "string"
}
```

**Responses**:
- **201**: Created
  ```json
  {
    "id": "uuid",
    "description": "string",
    "tagType": "string"
  }
  ```
- **400**: Bad Request
  ```json
  {
    "type": "string",
    "title": "string",
    "status": 0,
    "detail": "string",
    "instance": "string"
  }
- **401**: Unauthorized
  ```json
  {
    "type": "string",
    "title": "string",
    "status": 0,
    "detail": "string",
    "instance": "string"
  }
- **500**: Server Error

### Users Endpoints

#### Register User

**Endpoint**: `/users/register`

**Method**: `POST`

**Tags**: Users

**Request Payload**:
```json
{
  "email": "string",
  "password": "string"
}
```

**Responses**:
- **200**: Success
- **400**: Bad Request
  ```json
  {
    "type": "string",
    "title": "string",
    "status": 0,
    "detail": "string",
    "instance": "string"
  }
- **409**: Conflict
  ```json
  {
    "type": "string",
    "title": "string",
    "status": 0,
    "detail": "string",
    "instance": "string"
  }
- **500**: Server Error

## Terraform - GCP

### Initial Setup
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

### Terraform development

* Must have the Terraform CLI installed.
* Must have a credentials file for the service account in the `/terraform` directory named `credentials.json` (**DO NOT COMMIT TO GIT**)
* Run `terraform init` to connect to the GCP backend tfstate
* Make your changes, run `terraform plan` and set your env variable in the command line based on the environment you're developing for
* If the plan is as expected, run `terraform apply` to deploy your changes

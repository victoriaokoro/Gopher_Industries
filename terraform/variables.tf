variable "env" {
    default = "dev"
}

variable "region" {
  default = "australia-southeast2"
}

variable "project" {
  default = "sit-23t2-food-remedy-7f9b4c0"
}

variable "foodremedy_database_root_password" {
  sensitive = true
  description = "Must assign value at deployment time. Do not hardcode in Terraform."
}

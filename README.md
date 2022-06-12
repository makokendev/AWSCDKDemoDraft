# Coding Challenge - SERVERLESS 

Main goal of this project is to demonstrate how a .NET solution can contain all the code that is needed to define, build & deploy a Serverless Application hosted on AWS. 

The overall architecture is shown in the [Overview diagram](http://awscdkdemo.makoken.com/docs/overview.html). 

## Highlights
* This project is based on the Coding Challange project detailed in the [BlockChain Event Stream Processor Coding Challenge](https://github.com/makokendev/BlockChainEventStreamProcessorCodingChallenge) github repository.

* [AWS CDK](https://aws.amazon.com/cdk/) framework & [CloudFormation](https://aws.amazon.com/cloudformation/) is used as the Infrastructure as Code (IaC) tool to be able to create relevant infrastructure on AWS.

* [Cake Buid](https://cakebuild.net/) is used as the build automation system. 

* [Github actions](https://github.com/features/actions) is used for CICD. 

* [AWS](https://aws.amazon.com/) is the cloud platform where the solution is realised. 
  * Cloud Native (serverless) components are used such a [SNS](https://aws.amazon.com/sns/), [SQS](https://aws.amazon.com/sns/), [DynamoDB](https://aws.amazon.com/dynamodb/), [API Gateway](https://aws.amazon.com/api-gateway/).

### Cake Build/Frosting

[Build Project](https://github.com/makokendev/AWSCDKDemoDraft/tree/master/build) includes set of tasks and utilities to enable build and deploy pipelines. The tasks are designed to be generic and driven by [Project Settings](https://github.com/makokendev/AWSCDKDemoDraft/blob/master/build/Tasks/SetupTask.cs). Dedicated Nuget packages can be created to include all the generic tasks and utilities so that multiple projects can leverage the basic tools and can focus on the configuration bit.

#### Build
* In this particular cake build project, the purpose of the build pipeline is to produce artifacts which can be deployed to desired platform e.g. AWS.
* The buid pipeline makes sure:
  * A version is determined for the specific version of the code based on git tags & commits
  * Dotnet code is built and all the tests passes.
  * A docker image is created and pushed to private Docker Registry (ECR).
  * "./build/build.sh --target=build" command triggers the build pipeline.

#### Deploy
* In this particular cake build project, the purpose of the deploy pipeline is to create relevant serverless infrastructure using CF if they do not exist and create/update lambda function(s) with the latest corresponding docker images. 
* This pipeline is created with the Continuous Deployment in mind. For Continuous Delivery, a separate deployment artifact can be generated.
* "./build/build.sh --target=deploy" command triggers the deploy pipeline.

### AWS CDK

* This solution contains a single cdk project which contains definitions for three stacks. The stacks can also be defined in separate projects but this setup is chosen to showcase the multi-stack support capability.
* Infra Stack must be deployed first. 
* Database Stack is deployed after infra stack. 
* Main/Application Stack is deployed last as it has dependencies on the Infra & Database stacks.

```
# infra stack
./build/build.sh --target=infradeploy
# database stack
./build/build.sh --target=databasedeploy
# main/application stack
./build/build.sh --target=deploy

```

#### Infra Stack

* Infra Stack's purpose is to create AWS resources that are used platform-wide such as ECR Repo, SNS Event Topic and S3 Bucket for storing documentation.

#### Database Stack

* It is a good practice to separate stacks for database and main application when working with CloudFormation as they usually have different lifecycles. We don't always want to rollback a database configuration when rollbacking an application version. 

#### Main Stack

* Main stack can also be called the Application stack as it contains specific resource definitions for an application. In this case, the application consists of a SQS FIFO Queue, Event/Queue Processor Lambda and Api Gateway instance. 
* Most of the changes typically occur in the Main/Application stack as new commands, queries, business rules etc are added to the application logic. 

## Interaction

### Send Transaction (Publish Transaction Event)
```
curl --location --request GET 'https://awscdkapi.makoken.com/topic?message=%7B%22Type%22%3A%22Mint%22%2C%22TokenId%22%3A%220xD000000000000000000000000000000000000000%22%2C%22Address%22%3A%220x2000000000000000000000000000000000000000%22%7D'
```
### Get tokens from a wallet
```
curl --location --request GET 'https://awscdkapi.makoken.com/wallet?walletid=0x1000000000000000000000000000000000000000'
```
### Get token by tokenid
```
curl --location --request GET 'https://awscdkapi.makoken.com/token?id=0xE000000000000000000000000000000000000000'
```
## Contact 

Please write to makoken@gmail.com for any remarks and questions.

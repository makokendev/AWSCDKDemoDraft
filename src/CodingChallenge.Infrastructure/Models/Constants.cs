namespace CodingChallenge.Infrastructure;

public static class Constants
{
    public const string APPLICATION_ENVIRONMENT_VAR_PREFIX = "APPLICATION";
    public const string REST_API_CNAME = "restapicname";
    public const string MAIN_STACK_NAME_SUFFIX = "mainstack";
    public const string INFRA_STACK_NAME_SUFFIX = "infrastack";

    public static string DATABASE_STACK_NAME_SUFFIX = "database";

    public const string AWS_ACCOUNT_NUMBER_ENV_NAME = "AWS_ACCOUNT_NUMBER";

    public const string AWS_REGION_ENV_NAME = "AWS_REGION";

    public const string DATABASE_TYPE_ENV_VAR_KEY = "DATABASE_TYPE";
    public const string DATABASE_TYPE_DYNAMODB_ENV_VAR_KEY = "DYNAMODB";
    public const string DATABASE_TYPE_SQLITE_ENV_VAR_KEY = "SQLITE";

}
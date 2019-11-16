# Authorization
Permission based authorization

## Basic usage
You can define one or more different enums containing all permissions used in your application. 
If you can build modular application, then every moduel can use own permission enum.
```
[Resource("Companies", "Company management")]
public enum CompanyPermission
{
    [StaticPermission("User can create new company")]
	Create = 1,
	
    [StaticPermission("User can view all companies")]
	ViewAll = 2,
}
```
Advice: Always specify numeric representation because it will be used as key in your data storage

### Register services for Dependency Injection
Specify your primary key type and implement IPermissionStore for data persistence. 
```
services.AddPermissions<long, AppPermissionStore>();
```
Register your permission enum and configure resource id. 
This id will be used for computing permission unique identifier. Every permission resource (enum) must be using own unique resource id
```
services.AddPermissionResource<CompanyPermission>(1);
```

#### Configure with authentication
To make authentication and authorization working together you will have to implement IClaimPrincipalProvider. 
If you are using web application, it just need to access ClaimPrincipal available from IHttpContextAcessor. 
Then use method RoleToClaim when you will be adding roles to user identity.

### Use in application
Inject IUser interface and check for permissions
```
_user.IsAllowed(CompanyPermission.Create);
// OR
_user.CheckPermission(CompanyPermission.Create);
```

## Specify permission for different data(enities)
Lets define company permission for CRUD operations.
```
[Resource("Companies", "Company management")]
public enum CompanyPermission
{
    [StaticPermission("User can create new company")]
    Create = 1,
    [InstancePermission("User can view company")]
    View = 2,
    [InstancePermission("User can edit company")] // Creates also static permission, whic hwill be fallbacked if this permission is not set
    Update = 3,
    [InstancePermission("User can delete company")]
    Delete = 4,

    [StaticPermission("Fire all employes", "Super cool translatable description for not so smart users")]
    FireAllEmployes = 5,
}
```
Implement data store for reading Id, Name and Description for company from interface IResourceInstanceProvider. 
Replace registration method AddPermissionResource in ServiceCollection by AddPermissionResourceForInstances
```
services.AddPermissionResourceForInstances<CompanyPermission, CompanyRepository>(1);
```

### Use in application
Inject IUser interface and check for permissions
```
_user.IsAllowed(CompanyPermission.Create, companyId);
// OR
_user.CheckPermission(CompanyPermission.Create, companyId);
```
In both case if this permission is not set, then it will be fallbacked to the same permission without specified ID


## Manage permissions
You can implement UI with all data provided by IPermissionManager. Or you can use very easy tool from project Pipaslot.AuthorizationUI for your administrators. 
This tool only needs to be registered in dependency injection:
```
 services.AddAuthorizationUI<long, PermissionStore>(3); // Number 3 is unique resource identifier because this feature uses the same kind of authorization
```
And register middleware
```
app.UseAuthorizationUI<long>(options =>
   {
       options.RoutePrefix = "security";
   });
```
then UI will be available on address http://my.domain/security
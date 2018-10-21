# DataTable-ServerSide-Implementation-Sample
This project is an example serverside implementation of datatable plugin (https://datatables.net/) in dotnet core, 
It is one simple CRUD application together with the datatable implementation, 
it facilitates generics, and expressions to get the most out of C#, the goal is to get dynamically built IQuerable, 
provided any datatable options for any object type, a dynamic query will be built.
It use EF Core and dotnet Core. 


## How it works
Datatable plugin will send the options to the server, based on these [options](https://datatables.net/examples/server_side/post.html) (which is defined by you) the server 
will generate the according [IQueryable](https://www.developerhandbook.com/entity-framework/in-the-spotlight-demystifying-iqueryable-entity-framework-6/) which will execute at the database , it will automatically do the searching 
ordering, and pagination specified by the options sent from the datatable, by leveraging [expression builders](https://blogs.msdn.microsoft.com/meek/2008/05/02/linq-to-entities-combining-predicates/) read more about expression builders [here]
(https://stackoverflow.com/questions/35346630/creating-dynamic-expression-for-entity-framework) and [here](https://www.c-sharpcorner.com/UploadFile/c42694/dynamic-query-in-linq-using-predicate-builder/). 

## Datatable Options (View side)
First you need to tell the plugin that it will be processed by the server by assigning these 2 options to true, and assign the url to the options 

                       `                                                                                                                        
                       processing: true,                                                                                
                        serverSide: true,                                                                                                                 
                        ajax: {                                                                                                         
                            url: "@Url.Action("GetDTResponseAsync")",                                                     
                            type: "POST",                                                         
                            error: function (ex) {                                    
                                                 }
                            `
                            
                            
Now you need to specify columns options, the source of each columns, and is it searchable and orderable or not. 
let's walk through these options 
### Columns 
                          `
                          columns: [                                                                                                                       
                            { data: "id", title: "NO", width: "50px", class: "text-center" },                                   
                            { data: "title", title: "TITLE", width: "20%", class: "text-center" },                              
                            { data: "mainCategory.name", title: "MainCategory", width: "20%", class: "text-center" },                       
                            { data: "subCategory.name", title: "SubCategory", width: "20%", class: "text-center" },                         
                            { data: "", defaultContent: '', title: "Modify", width: "15%", class: "text-center" }                     
                        ]
                        `
                        
as you can see, here we specify the data property for each column, which is the source or some of you might call it navigation property or attribute of the row, title will be the cell's header text.
### columnDefs
                        
                        `
                        columnDefs: [
                            { "orderable": false, "targets": [-1] },                                                                  
                            { "searchable": false, "targets": [-1] },
                            { "orderable": true, "targets": [0,1,2,3] },
                            { "searchable": true, "targets": [0,1,2,3] }
                            
                            `
                            
                            
Here we specify which columns are searchable or not using the index of the columns, ex 0 = id, and so on. 
you can find the sample under \Views\Home\Index.cshtml. 

## Serverside function
This function will accept the datatable options which will be sent by the plugin, you can have a look at [Options class](https://github.com/rezres/DataTable-ServerSide-Implementation-Sample/blob/master/DataTable%20ServerSide%20%20Implementation%20Sample/Data/Requests/DataTableOptions.cs) if you are interested. 
Now you can either use the repo I created, and use GetOptionResponseWithSpec Function which will return [Datatable Response Object](https://github.com/rezres/DataTable-ServerSide-Implementation-Sample/blob/master/DataTable%20ServerSide%20%20Implementation%20Sample/Data/Responses/DataTableResponse.cs)
all you need to assign is the type, any expressions you want to include (Navigation Properties)
Below is an example you can find in HomeController\GetDTResponseAsync. 


If you don't want to use the Repository, all you need to do is actually use the [GetOptionResponseAsync] Function, you can find it at [Datatable Helper class](https://github.com/rezres/DataTable-ServerSide-Implementation-Sample/blob/master/DataTable%20ServerSide%20%20Implementation%20Sample/Extensions/DataTableHelper.cs)
, this function will generate the IQuerable and execute it, and wrap the response, it is an extension method of IQuerable.
You can use it like this. 


`
await context.Set<T>().GetOptionResponseAsync<T>(options);
`


of course keep in mind adding navigation properties (Include) if you have any.  

I hope this was a good readme file,
please don't hesitate to ask me anything @ 94munzernasr@gmail.com
 


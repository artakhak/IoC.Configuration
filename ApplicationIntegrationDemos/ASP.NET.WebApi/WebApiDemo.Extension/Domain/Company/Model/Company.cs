namespace WebApiDemo.Extension.Domain.Company.Model;

public class Company
{
    public string Name { get; set; } = null!;
    public string CEO { get; set; } = null!;
}

/*
"CompanyData": {
    "Name": "Strange Things, Inc",
    "CEO": "John Malkowich",
    "Address": {
        "Street": "123 Maple Street",
        "City": "Springfield",
        "State": "IL",
        "ZipCode": "62701"
    },
    "CountryDetails": "$value(Countries.Where(x => x.Name == 'United States'))"
},*/
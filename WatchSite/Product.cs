using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchSite
{
    class Product
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Price { get; set; }
        public string Url { get; set; }
        public string Desc { get; set; }

    }
    class ProductComparer : IEqualityComparer<Product>
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(Product x, Product y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.Id == y.Id;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(Product product)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(product, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashProductId = product.Id == null ? 0 : product.Id.GetHashCode();

           //Calculate the hash code for the product.
            return hashProductId;
        }

    }

    class ProductComparerPrice : IEqualityComparer<Product>
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(Product x, Product y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.Price == y.Price;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(Product product)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(product, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashProductPrice = product.Price == null ? 0 : product.Price.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductPrice;
        }

    }

}

﻿using Bulky.DataAccess.Repository.IRepository;
using BulkyBook.DataAccess.Repository.IRepository;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using System.Numerics;

namespace BulkyBook.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDBContext _db;
    public ICategoryRepository Category { get; private set; }
    public IProductRepository Product { get; private set; }
    public ICompanyRepository Company { get; private set; }
    public IShoppingCartRepository ShoppingCart { get; private set; }
    public UnitOfWork(ApplicationDBContext db)
    {
        _db = db;
        Category = new CategoryRepository(_db);
        Product = new ProductRepository(_db);
        Company = new CompanyRepository(_db);
        ShoppingCart = new ShoppingCartRepository(_db);
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}

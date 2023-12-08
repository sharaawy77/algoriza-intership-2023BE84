using CoreLayer.Interface;
using EFLayer.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLayer.Repos
{
    public class BaseRepo<T> : IBaseRepo<T> where T : class
    {
        private readonly ApplicationDbContext context;

        public BaseRepo(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> CreateAsync(T entity)
        {
            try
            {
                await context.Set<T>().AddAsync(entity);
                var res = await context.SaveChangesAsync();
                if (res != 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                return false;
            }
            
        }
        
        public async Task<bool> Delete(string id)
        {
            try
            {
                var entity = await GetByidAsync(id);
                context.Remove(entity);

                var res = await context.SaveChangesAsync();
                if (res != 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                return false;
            }
           
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var entity = await GetByidAsync(id);
                context.Remove(entity);
                var res = await context.SaveChangesAsync();
                if (res != 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                return false;
            }
            
        }

        public  IQueryable<T> GetAll()
        {
            try
            {
                return context.Set<T>().AsQueryable();

            }
            catch (Exception)
            {

                return null;
            }
        }

        public async Task<T> GetByidAsync(string id)
        {
            try
            {
                if (id != null)
                {
                    return await context.Set<T>().FindAsync(id);

                }
                return null;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public async Task<T> GetByidAsync(int id)
        {
            try
            {
                if (id != null)
                {
                    return await context.Set<T>().FindAsync(id);

                }
                return null;
            }
            catch (Exception)
            {

                return null;
            }

        }

        public async Task<T> GetByidAsync(int? id)
        {
            try
            {
                if (id != null)
                {
                    return await context.Set<T>().FindAsync(id);

                }
                return null;
            }
            catch (Exception)
            {

                return null;
            }
            
        }

        public async Task<T> Update(T entity)
        {
            try
            {
                context.Update(entity);
                await context.SaveChangesAsync();
                return entity;
            }
            catch (Exception)
            {

                return null;
            }
            
        }
    }
}

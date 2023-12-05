//using CoreLayer.Interface;
//using EFLayer.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace EFLayer.Repos
//{
//    public class UnitOfWork : IUnitOfWork
//    {
//        private readonly ApplicationDbContext context;

//        public UnitOfWork(ApplicationDbContext context)
//        {
//            this.context = context;
//        }
//        public int Complete()
//        {
//            return context.SaveChanges();
//        }

//        public void Dispose()
//        {
//            context.Dispose();
//        }
//    }
//}

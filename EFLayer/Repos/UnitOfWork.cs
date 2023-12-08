using CoreLayer.Interface;
using CoreLayer.Models;
using EFLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLayer.Repos
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;

        public UnitOfWork(ApplicationDbContext context)
        {
            
            this.context = context;
            docrepo = new BaseRepo<Doctor>(context);
            patrepo = new BaseRepo<Patient>(context);
            apprepo = new BaseRepo<Appointment>(context);
            bookrepo = new BaseRepo<Booking>(context);
            specrepo = new BaseRepo<Specialization>(context);
            disrepo = new BaseRepo<DiscoundCode>(context);
            timrepo = new BaseRepo<Time>(context);
        }
        public IBaseRepo<Doctor> docrepo { get; private set; }

        public IBaseRepo<Patient> patrepo { get; private set; }

        public IBaseRepo<Appointment> apprepo { get; private set; }

        public IBaseRepo<Booking> bookrepo { get; private set; }

        public IBaseRepo<Specialization> specrepo { get; private set; }

        public IBaseRepo<DiscoundCode> disrepo { get; private set; }

        public IBaseRepo<Time> timrepo { get; private set; }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}

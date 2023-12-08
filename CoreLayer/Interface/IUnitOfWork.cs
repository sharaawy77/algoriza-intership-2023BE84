using CoreLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepo<Doctor> docrepo { get; }
        IBaseRepo<Patient> patrepo { get; }
        IBaseRepo<Appointment> apprepo { get; }
        IBaseRepo<Booking> bookrepo { get; }
        IBaseRepo<Specialization> specrepo { get; }
        IBaseRepo<DiscoundCode> disrepo { get; }
        IBaseRepo<Time> timrepo { get; }




    }
}

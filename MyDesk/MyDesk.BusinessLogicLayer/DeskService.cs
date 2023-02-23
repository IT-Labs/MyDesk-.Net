using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyDesk.Core.Database;
using MyDesk.Core.DTO;
using MyDesk.Core.Entities;
using MyDesk.Core.Exceptions;
using MyDesk.Core.Interfaces.BusinessLogic;

namespace MyDesk.BusinessLogicLayer
{
    public class DeskService : IDeskService
    {
        private readonly IMapper _mapper;
        private readonly IContext _context;

        public DeskService(IMapper mapper,
            IContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public List<DeskDto> GetOfficeDesks(int id, int? take = null, int? skip = null)
        {
            
            var query = _context
                .AsQueryable<Desk>()
                .Where(x => x.OfficeId == id && x.IsDeleted == false)
                .Include(x => x.Categorie)
                .Include(x => x.Reservations.Where(y => y.IsDeleted == false))
                    .ThenInclude(x => x.Employee);

            var desks = (take.HasValue && skip.HasValue) ?
                query.Skip(skip.Value).Take(take.Value).ToList() :
                query.ToList();

            return _mapper.Map<List<DeskDto>>(desks);
        }

        public void Create(int officeId, int numberOfInstancesToCreate)
        {
            var existinOffice = _context
                .AsQueryable<Office>()
                .FirstOrDefault(x => x.Id == officeId && x.IsDeleted == false);

            if (existinOffice == null)
            {
                throw new NotFoundException($"Office with ID: {officeId} not found.");
            }

            var highestIndex = _context
                .AsQueryable<Desk>()
                .Where(x => x.OfficeId == officeId && x.IsDeleted == false)
                .OrderByDescending(x => x.IndexForOffice)
                .Select(x => x.IndexForOffice)
                .FirstOrDefault() ?? 0;

            var desksToInsert = new List<Desk>();

            for (int i = 0; i < numberOfInstancesToCreate; i++)
            {
                var desk = new Desk()
                {
                    OfficeId = officeId,
                    IsDeleted = false,
                    Categories = "regular",
                    IndexForOffice = highestIndex + i + 1
                };

                desksToInsert.Add(desk);
            }

            _context.Insert(desksToInsert);
        }

        public void Update(List<DeskDto> desks)
        {
            using (var transaction = _context.BeginTransaction() as IDbContextTransaction)
            {
                try
                {
                    foreach (var deskToUpdate in desks)
                    {
                        var desk = _context
                            .AsQueryable<Desk>()
                            .FirstOrDefault(x => deskToUpdate.Id.HasValue && x.Id == deskToUpdate.Id.Value && x.IsDeleted == false);
                        if (desk == null)
                        {
                            continue;
                        }

                        var deskToUpdateCategory = deskToUpdate.Category;
                        if (deskToUpdateCategory == null)
                        {
                            throw new NotFoundException($"DeskToUpdate.Category is null.");
                        }

                        var existingDeskCategory = _context
                            .AsQueryable<Category>()
                            .FirstOrDefault(x => x.DoubleMonitor == deskToUpdateCategory.DoubleMonitor && 
                                x.NearWindow == deskToUpdateCategory.NearWindow &&
                                x.SingleMonitor == deskToUpdateCategory.SingleMonitor && 
                                x.Unavailable == deskToUpdateCategory.Unavailable && 
                                x.IsDeleted == false);
                        if (existingDeskCategory != null)
                        {
                            desk.CategorieId = existingDeskCategory.Id;
                            _context.Modify(desk);
                        }
                        else
                        {
                            Category category = new Category()
                            {
                                DoubleMonitor = deskToUpdateCategory.DoubleMonitor,
                                SingleMonitor = deskToUpdateCategory.SingleMonitor,
                                NearWindow = deskToUpdateCategory.NearWindow,
                                Unavailable = deskToUpdateCategory.Unavailable
                            };

                            _context.Insert(category);
                            desk.CategorieId = category.Id;
                            _context.Modify(desk);
                        }
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }

                transaction.Commit();
            }
        }

        public void Delete(int id)
        {
            var desk = _context
                .AsQueryable<Desk>()
                .Include(x => x.Reservations.Where(y => y.IsDeleted == false))
                    .ThenInclude(x => x.Reviews.Where(y => y.IsDeleted == false))
                .FirstOrDefault(x => x.Id == id && x.IsDeleted == false);
            if (desk == null)
            {
                throw new NotFoundException($"Desk with ID: {id} not found.");
            }

            foreach (Reservation reservation in desk.Reservations)
            {
                reservation.IsDeleted = true;
                foreach (var review in reservation.Reviews)
                {
                    review.IsDeleted = true;
                }
            }

            desk.IsDeleted = true;
            _context.Modify(desk);
        }
    }
}

﻿using inOfficeApplication.Data.Entities;

namespace inOffice.Repository.Interface
{
    public interface IOfficeRepository
    {
        Office Get(int id, bool? includeDesks = null, bool? includeConferenceRooms = null);
        Office GetByName(string name);
        List<Office> GetAll(int? take = null, int? skip = null);
        void Insert(Office office);
        void Update(Office office);
        void SoftDelete(Office office);
    }
}

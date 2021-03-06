using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NMS_API_N.Model.DTO;
using NMS_API_N.Model.Entities;
using NMS_API_N.Extension;
using NMS_API_N.Helper;
using NMS_API_N.Unit_Of_Work;

namespace NMS_API_N.Controllers
{
    [Authorize(Roles = "admin,management")]
    public class AddressController : BaseApiController
    {
        private readonly IUnitOfWork _uot;
        private readonly IMapper _mapper;

        public AddressController(IUnitOfWork uot, IMapper mapper)
        {
            _uot = uot;
            _mapper = mapper;
        }

        [HttpPost("add-address")]
        public async Task<ActionResult<AddressDto>> AddAddress(AddressDto addressDto)
        {
            var addressData = _mapper.Map<Address>(addressDto);

            var checkAddress = await _uot.AddressRepository.CheckAddress(addressData.SourceId, addressData.SourceType, addressData.CityId);

            if (checkAddress != null) return BadRequest("Address for this source already exist");

            addressData.AddressDescription = addressDto.AddressDescription;
            addressData.SourceType = addressDto.SourceType.ToLower();
            addressData.CreatedBy = int.Parse(User.GetUserId());

            _uot.AddressRepository.AddAddress(addressData);

            if (await _uot.Complete())
                return Ok(addressData);

            return BadRequest("Failed To Add Address");
        }

        [HttpPut("update-address")]
        public async Task<ActionResult> UpdateAddress(AddressDto addressDto)
        {
            var addressData = await _uot.AddressRepository.GetAddressByIdAsynch(addressDto.AddressId);

            if (addressData == null) return BadRequest("No Record Found");

            if (IsSameAddressInfo(addressDto, addressData)) return BadRequest("You did not change anything for update");

            var data = _mapper.Map(addressDto, addressData);

            data.UpdatedBy = int.Parse(User.GetUserId());
            data.LastUpdatedDate = DateTime.Now;
            data.UpdatedCount += 1;

            _uot.AddressRepository.UpdateAddress(data);

            if (await _uot.Complete()) 
                return NoContent();

            return BadRequest("Failed to Update Address");
        }

        private bool IsSameAddressInfo(AddressDto addressDto, Address address)
        {
            ObjectTrimmer.MakeObjectTrim(addressDto);

            return addressDto.AddressDescription == address.AddressDescription &&
                   addressDto.SourceId == address.SourceId &&
                   addressDto.SourceType.ToLower() == address.SourceType &&
                   addressDto.Phone == address.Phone &&
                   addressDto.CityId == address.CityId;
        }

    }
}

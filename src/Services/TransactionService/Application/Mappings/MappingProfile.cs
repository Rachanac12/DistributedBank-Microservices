using AutoMapper;
using TransactionService.Application.Commands;
using TransactionService.DTOs;
using TransactionService.Models;

namespace TransactionService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateTransactionCommand, Transaction>();
            CreateMap<Transaction, TransactionDto>();
        }
    }
}

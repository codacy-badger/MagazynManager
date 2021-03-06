﻿using MagazynManager.Application.Commands.Slowniki;
using MagazynManager.Application.QueryHandlers;
using MagazynManager.Domain.Entities.Produkty;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MagazynManager.Application.CommandHandlers.Slowniki
{
    [CommandHandler]
    public class ProduktCommandHandler : IRequestHandler<ProduktCreateCommand, Guid>,
        IRequestHandler<ProduktDeleteCommand, Unit>
    {
        private readonly IProduktRepository _produktRepository;
        private readonly IKategoriaRepository _kategoriaRepository;
        private readonly IJednostkaMiaryRepository _jednostkaMiaryRepository;

        public ProduktCommandHandler(IProduktRepository produktRepository, IKategoriaRepository kategoriaRepository, IJednostkaMiaryRepository jednostkaMiaryRepository)
        {
            _produktRepository = produktRepository;
            _kategoriaRepository = kategoriaRepository;
            _jednostkaMiaryRepository = jednostkaMiaryRepository;
        }

        public async Task<Guid> Handle(ProduktCreateCommand request, CancellationToken cancellationToken)
        {
            var kategorie = await _kategoriaRepository.GetList(request.PrzedsiebiorstwoId);
            var kategoria = kategorie.FirstOrDefault(x => x.Nazwa == request.Kategoria);

            if (kategoria == null)
            {
                kategoria = new Kategoria(request.Kategoria, request.PrzedsiebiorstwoId);
                await _kategoriaRepository.Save(kategoria);
            }

            var jednostkiMiary = await _jednostkaMiaryRepository.GetList(request.PrzedsiebiorstwoId);
            var jednostkaMiary = jednostkiMiary.FirstOrDefault(x => x.Nazwa == request.JednostkaMiary);

            if (jednostkaMiary == null)
            {
                jednostkaMiary = new JednostkaMiary(request.JednostkaMiary, request.PrzedsiebiorstwoId);
                await _jednostkaMiaryRepository.Save(jednostkaMiary);
            }

            var produkt = new Produkt
            {
                Id = Guid.NewGuid(),
                Skrot = request.ShortName,
                Nazwa = request.Name,
                Kategoria = kategoria,
                JednostkaMiary = jednostkaMiary,
                MagazynId = request.MagazynId
            };

            return await _produktRepository.Save(produkt);
        }

        public async Task<Unit> Handle(ProduktDeleteCommand request, CancellationToken cancellationToken)
        {
            await _produktRepository.Delete(request.ProduktId);
            return Unit.Value;
        }
    }
}
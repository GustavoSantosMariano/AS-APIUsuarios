public class UsuarioUpdateDtoValidator : AbstractValidator<UsuarioUpdateDto>
{
    public UsuarioUpdateDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.DataNascimento)
            .NotEmpty()
            .Must(d => d <= DateTime.Today.AddYears(-18))
            .WithMessage("Usuário deve ser maior de 18 anos.");

        RuleFor(x => x.Telefone)
            .Matches(@"^\(\d{2}\) \d{5}-\d{4}$")
            .When(x => !string.IsNullOrEmpty(x.Telefone));
    }
}
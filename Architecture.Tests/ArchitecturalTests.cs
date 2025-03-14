using FluentAssertions;
using NetArchTest.Rules;

namespace ArchitectureTests;

public class ArchitecturalTests
{
    private const string DomainNamespace = "Domain";
    private const string ApplicationNamespace = "Application";
    private const string InfrastructureNamespace = "Infrastructure"; // Fixed spelling
    private const string PresentationNamespace = "Presentation";
    private const string WebNamespace = "Web";

    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = typeof(Domain.AssemblyReference).Assembly;

        string[] otherProjects =
        [
            ApplicationNamespace,
                InfrastructureNamespace,
                PresentationNamespace,
                WebNamespace
        ];

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = typeof(Application.AssemblyReference).Assembly;

        string[] otherProjects =
        [
            InfrastructureNamespace,
            PresentationNamespace,
            WebNamespace
        ];

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Handlers_Should_Have_DependencyOnDomain()
    {
        // Assert
        var assembly = typeof(Application.AssemblyReference).Assembly;
        
        // Act
        var testResult = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .HaveDependencyOn(DomainNamespace)
            .GetResult();
        
        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = typeof(Infrastructure.AssemblyReference).Assembly;

        string[] otherProjects =
        [
            ApplicationNamespace,
            WebNamespace
        ];

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Presentation_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = typeof(Presentation.AssemblyReference).Assembly;

        string[] otherProjects =
        [
            InfrastructureNamespace,
            WebNamespace
        ];

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Controller_Should_HaveDependencyOnMediatR()
    {
        // Arrange
        var assembly = typeof(Presentation.AssemblyReference).Assembly;

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Controller")
            .Should()
            .HaveDependencyOn("MediatR")
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }
}

namespace DWD.UI.PredicateBuilder.Tests;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;

public sealed class PredicateBulderTest : IDisposable
{
    private readonly AnEntityContext dbContext;

    public PredicateBulderTest()
    {
        this.dbContext =
            new AnEntityContext(new DbContextOptionsBuilder<AnEntityContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        _ = this.dbContext.Add(new AnEntity() { Id = 1, Fk = 1, P1 = 1, P2 = 1 });
        _ = this.dbContext.Add(new AnEntity() { Id = 2, Fk = 1, P1 = 1, P2 = 2 });
        _ = this.dbContext.Add(new AnEntity() { Id = 3, Fk = 1, P1 = 1, P2 = 3 });
        _ = this.dbContext.Add(new AnEntity() { Id = 4, Fk = 2, P1 = 1, P2 = 3 });
        _ = this.dbContext.SaveChanges();
    }

    [Fact]
    public void ShouldBuildQueryWithMultipleOrPredicates()
    {
        // Arrange
        const int queryFk = 1;
        var queryObjects = new Collection<AnEntityQueryObject>
            {
                new AnEntityQueryObject() {P1 = 1, P2 = 2},
                new AnEntityQueryObject() {P1 = 1, P2 = 3},
            };

        // Act
        var result = this.SetUpAndRunQuery(queryFk, queryObjects);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(2, result[0].Id);
        Assert.Equal(3, result[1].Id);
    }

    [Fact]
    public void ShouldBuildQueryWithMultipleOrPredicatesWhenNoMatches()
    {
        // Arrange
        const int queryFk = 1;
        var queryObjects = new Collection<AnEntityQueryObject>
            {
                new AnEntityQueryObject() {P1 = 2, P2 = 2},
                new AnEntityQueryObject() {P1 = 2, P2 = 3},
            };

        // Act
        var result = this.SetUpAndRunQuery(queryFk, queryObjects);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ShouldBuildQueryWithMultipleOrPredicatesWhenNoFkMatches()
    {
        // Arrange
        const int queryFk = 3;
        var queryObjects = new Collection<AnEntityQueryObject>
            {
                new AnEntityQueryObject() {P1 = 1, P2 = 2},
                new AnEntityQueryObject() {P1 = 1, P2 = 3},
            };

        // Act
        var result = this.SetUpAndRunQuery(queryFk, queryObjects);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    private System.Collections.Generic.List<AnEntity> SetUpAndRunQuery(int queryFk, Collection<AnEntityQueryObject> queryObjects)
    {
        var fkExpression = PredicateBuilder.Create<AnEntity>(p => p.Fk == queryFk);
        var orExpression = PredicateBuilder.False<AnEntity>();
        foreach (var queryObject in queryObjects)
        {
            orExpression = orExpression.Or(p => p.P1 == queryObject.P1 && p.P2 == queryObject.P2);
        }
        var result = this.dbContext.AnEntities.Where(fkExpression.And(orExpression)).ToList();
        return result;
    }

    public void Dispose() => this.dbContext.Dispose();
}

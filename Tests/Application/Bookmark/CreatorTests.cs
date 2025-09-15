using Application.Bookmark;
using Application.Bookmark.Commands;
using Domain.Entities;
using Domain.Shared;

namespace Tests.Application.Bookmark;

[TestClass]
public partial class CreatorTests
{
    private FakeTagRepository tagRepo = null!;
    private FakeBookmarkRepository bookmarkRepo = null!;
    private Creator creator = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        tagRepo = new FakeTagRepository();
        bookmarkRepo = new FakeBookmarkRepository();
        creator = new Creator(tagRepo, bookmarkRepo);
    }

    [TestMethod]
    public async Task DoAsync_ReturnsFailure_When_UrlExists()
    {
        bookmarkRepo.UrlExists = true;
        bookmarkRepo.TitleExists = false;

        var result = await creator.DoAsync(new Create("http://a", "t", true));

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Bookmark.DuplicateUrl", result.Error.Code);
    }

    [TestMethod]
    public async Task DoAsync_ReturnsFailure_When_TitleExists()
    {
        bookmarkRepo.UrlExists = false;
        bookmarkRepo.TitleExists = true;

        var result = await creator.DoAsync(new Create("http://a", "t", true));

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Bookmark.DuplicateTitle", result.Error.Code);
    }

    [TestMethod]
    public async Task DoAsync_CreatesBookmark_When_NoDuplicates_And_NoTags()
    {
        bookmarkRepo.UrlExists = false;
        bookmarkRepo.TitleExists = false;

        var result = await creator.DoAsync(new Create("http://a", "t", true, "d", null));

        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(bookmarkRepo.CreateCalled);
        Assert.IsNotNull(bookmarkRepo.CreatedBookmark);
        Assert.AreEqual("http://a", bookmarkRepo.CreatedBookmark.Url);
        Assert.AreEqual("t", bookmarkRepo.CreatedBookmark.Title);
    }

    [TestMethod]
    public async Task DoAsync_CreatesTags_And_Bookmark_When_TagsProvided()
    {
        bookmarkRepo.UrlExists = false;
        bookmarkRepo.TitleExists = false;
        tagRepo.TagsToReturn = new List<Tag>();

        var result = await creator.DoAsync(new Create("http://a", "t", true, null, new[] { "tag1", "tag2" }));

        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(tagRepo.CreateCalled);
        Assert.IsTrue(bookmarkRepo.CreateCalled);
    }

    [TestMethod]
    public async Task DoAsync_ReturnsFailure_When_ExistsCheckFails()
    {
        bookmarkRepo.ExistsByUrlAsyncError = new Error("Repo.UrlCheckFail", "URL check failed");
        var result = await creator.DoAsync(new Create("http://a", "t", true));
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Error.Aggregate", result.Error.Code);
        Assert.IsTrue(result.Error.InnerErrors.Any(e => e.Code == "Repo.UrlCheckFail"));
    }

    [TestMethod]
    public async Task DoAsync_ReturnsFailure_When_TagFetchFails()
    {
        bookmarkRepo.UrlExists = false;
        bookmarkRepo.TitleExists = false;
        tagRepo.GetByNameAsyncError = new Error("TagRepo.FetchFail", "Tag fetch failed");
        var result = await creator.DoAsync(new Create("http://a", "t", true, null, new[] { "tag1" }));
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("TagRepo.FetchFail", result.Error.Code);
    }

    [TestMethod]
    public async Task DoAsync_ReturnsFailure_When_TagCreateFails()
    {
        bookmarkRepo.UrlExists = false;
        bookmarkRepo.TitleExists = false;
        tagRepo.FailTagCreateFor = "badtag";
        var result = await creator.DoAsync(new Create("http://a", "t", true, null, new[] { "badtag" }));
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Error.Aggregate", result.Error.Code);
        Assert.IsTrue(result.Error.InnerErrors.Any(e => e.Code == "Domain.Errors.Tag.InvalidName"));
    }

    [TestMethod]
    public async Task DoAsync_ReturnsFailure_When_TagPersistFails()
    {
        bookmarkRepo.UrlExists = false;
        bookmarkRepo.TitleExists = false;
        tagRepo.CreateAsyncError = new Error("TagRepo.PersistFail", "Tag persist failed");
        var result = await creator.DoAsync(new Create("http://a", "t", true, null, new[] { "tag1" }));
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("TagRepo.PersistFail", result.Error.Code);
    }

    [TestMethod]
    public async Task DoAsync_ReturnsFailure_When_BookmarkCreateFails()
    {
        bookmarkRepo.UrlExists = false;
        bookmarkRepo.TitleExists = false;
        bookmarkRepo.FailBookmarkCreate = true;
        var result = await creator.DoAsync(new Create("badurl", "t", true));
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Domain.Errros.Bookmark.InvalidUrl", result.Error.Code);
    }

    [TestMethod]
    public async Task DoAsync_ReturnsFailure_When_BookmarkPersistFails()
    {
        bookmarkRepo.UrlExists = false;
        bookmarkRepo.TitleExists = false;
        bookmarkRepo.CreateAsyncError = new Error("BookmarkRepo.PersistFail", "Bookmark persist failed");
        var result = await creator.DoAsync(new Create("http://a", "t", true));
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("BookmarkRepo.PersistFail", result.Error.Code);
    }
}

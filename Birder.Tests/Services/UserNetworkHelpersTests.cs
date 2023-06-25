﻿namespace Birder.Tests.Services;

public class UserNetworkHelpersTests
{
    [Fact]
    public void GetFollowersUserNames_ReturnsEmptyCollection_WhenInputCollectionIsEmpty()
    {
        // Arrange
        var sut = new UserNetworkHelpers();
        var emptyInputCollection = new List<Network>();

        // Act
        var result = sut.GetFollowersUserNames(emptyInputCollection);

        // Assert
        Assert.IsType<List<String>>(result);
        Assert.Empty(result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(59)]
    public void GetFollowersUserNames_ReturnsCollection_WhenInputCollectionLengthIsGreaterThan0(int length)
    {
        // Arrange
        var sut = new UserNetworkHelpers();
        var testCollection = GetDynamicNetworkCollection(length);

        // Act
        var result = sut.GetFollowersUserNames(testCollection);

        // Assert
        Assert.IsType<List<String>>(result);
        Assert.NotEmpty(result);
        Assert.Equal(length, result.Count);
        Assert.Equal("Test 1", result[0]);
        Assert.Equal("Test " + length.ToString(), result[length - 1]);
    }

    [Fact]
    public void GetFollowersUserNames_ReturnsNullReferenceException_WhenArgumentIsNull()
    {
        // Arrange
        var sut = new UserNetworkHelpers();

        // Act & Assert
        var ex = Assert.Throws<NullReferenceException>(() => sut.GetFollowersUserNames(null));
        Assert.Equal("The followers collection is null", ex.Message);
    }

    #region GetFollowingUserNames unit tests

    [Fact]
    public void GetFollowingUserNames_ReturnsEmptyCollection_WhenInputCollectionIsEmpty()
    {
        // Arrange
        var sut = new UserNetworkHelpers();
        var emptyInputCollection = new List<Network>();

        // Act
        var result = sut.GetFollowingUserNames(emptyInputCollection);

        // Assert
        Assert.IsType<List<String>>(result);
        Assert.Empty(result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(59)]
    public void GetFollowingUserNames_ReturnsCollection_WhenInputCollectionLengthIsGreaterThan0(int length)
    {
        // Arrange
        var sut = new UserNetworkHelpers();
        var testCollection = GetDynamicNetworkCollection(length);

        // Act
        var result = sut.GetFollowingUserNames(testCollection);

        // Assert
        Assert.IsType<List<String>>(result);
        Assert.NotEmpty(result);
        Assert.Equal(length, result.Count);
        Assert.Equal("Test 1", result[0]);
        Assert.Equal("Test " + length.ToString(), result[length - 1]);
    }

    [Fact]
    public void GetFollowingUserNames_ReturnsNullReferenceException_WhenArgumentIsNull()
    {
        // Arrange
        var sut = new UserNetworkHelpers();

        // Act & Assert
        var ex = Assert.Throws<NullReferenceException>(() => sut.GetFollowingUserNames(null));
        Assert.Equal("The following collection is null", ex.Message);
    }

    #endregion

    #region GetFollowersNotBeingFollowedUserNames unit tests

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(59)]
    public void GetFollowersNotBeingFollowedUserNames_ReturnsEmptyCollection_WhenInputCollectionIsEmpty(int length)
    {
        // Arrange
        var sut = new UserNetworkHelpers();
        var user = new ApplicationUser() { UserName = "Test User" };
        user.Following = GetDynamicNetworkCollection(length);
        user.Followers = new List<Network>();

        // Act
        var result = sut.GetFollowersNotBeingFollowedUserNames(user);

        // Assert
        Assert.IsAssignableFrom<IEnumerable<String>>(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetFollowersNotBeingFollowedUserNames_ReturnsCollection_WhenCollectionsAreGreaterThanOne()
    {
        // Arrange
        var sut = new UserNetworkHelpers();
        var user = new ApplicationUser() { UserName = "Test User" };
        user.Following = GetDynamicNetworkCollection(3);
        user.Followers = GetDynamicNetworkCollection(6);

        // Act
        var result = sut.GetFollowersNotBeingFollowedUserNames(user);

        // Assert
        var t = Assert.IsAssignableFrom<IEnumerable<String>>(result);
        Assert.Equal(3, t.Count());
    }

    [Fact]
    public void GetFollowersNotBeingFollowedUserNames_ReturnsNullReferenceException_WhenUserIsNull()
    {
        // Arrange
        var sut = new UserNetworkHelpers();

        // Act & Assert
        var ex = Assert.Throws<NullReferenceException>(() => sut.GetFollowersNotBeingFollowedUserNames(null));
        Assert.Equal("The user is null", ex.Message);
    }

    [Fact]
    public void GetFollowersNotBeingFollowedUserNames_ReturnsNullReferenceException_WhenFollowersCollectionIsNull()
    {
        // Arrange
        var sut = new UserNetworkHelpers();
        var user = new ApplicationUser() { UserName = "Test User" };
        user.Following = new List<Network>();
        user.Followers = null;

        // Act & Assert
        var ex = Assert.Throws<NullReferenceException>(() => sut.GetFollowersNotBeingFollowedUserNames(user));
        Assert.Equal("The followers collection is null", ex.Message);
    }

    [Fact]
    public void GetFollowersNotBeingFollowedUserNames_ReturnsNullReferenceException_WhenFollowingCollectionIsNull()
    {
        // Arrange
        var sut = new UserNetworkHelpers();
        var user = new ApplicationUser() { UserName = "Test User" };
        user.Following = null;
        user.Followers = new List<Network>();

        // Act & Assert
        var ex = Assert.Throws<NullReferenceException>(() => sut.GetFollowersNotBeingFollowedUserNames(user));
        Assert.Equal("The following collection is null", ex.Message);
    }

    #endregion

    #region UpdateIsFollowing unit tests

    [Fact]
    public void UpdateIsFollowing_ReturnsIsFollowEqualsTrue_WhenViewModelUserIsInFollowingCollection()
    {
        // Arrange
        var sut = new UserNetworkHelpers();
        var viewModel = new NetworkUserViewModel() { UserName = "Test 2" };
        var following = GetDynamicNetworkCollection(8);

        // Act
        var result = sut.UpdateIsFollowing(viewModel.UserName, following);

        // Assert
        var returnModel = Assert.IsType<bool>(result);
        //Assert.Equal(viewModel.UserName, returnModel.UserName);
        Assert.True(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(34)]
    public void UpdateIsFollowing_ReturnsIsFollowEqualsFalse_WhenViewModelUserIsNotInFollowingCollection(int length)
    {
        // Arrange
        var sut = new UserNetworkHelpers();
        var viewModel = new NetworkUserViewModel() { UserName = "User Not In Following Collection" };
        var following = GetDynamicNetworkCollection(length);

        // Act
        var result = sut.UpdateIsFollowing(viewModel.UserName, following);

        // Assert
        var returnModel = Assert.IsType<bool>(result);
        //Assert.Equal(viewModel.UserName, returnModel.UserName);
        Assert.False(result);
    }


    [Fact]
    public void UpdateIsFollowing_ReturnsNullReferenceException_WhenFollowingCollectionIsNull()
    {
        // Arrange
        var sut = new UserNetworkHelpers();
        var viewModel = new NetworkUserViewModel() { UserName = "Test User" };

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => sut.UpdateIsFollowing(viewModel.UserName, null));
        Assert.Equal("The following collection is null (Parameter 'requestedUsersFollowing')", ex.Message);
    }

    [Fact]
    public void UpdateIsFollowing_ReturnsNullReferenceException_WhenUsernameIsNull()
    {
        // Arrange
        var sut = new UserNetworkHelpers();
        var viewModel = new NetworkUserViewModel() { UserName = "Test User" };

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => sut.UpdateIsFollowing(null, new List<Network>()));
        Assert.Equal("The requestingUsername is null (Parameter 'requestingUsername')", ex.Message);
    }

    #endregion


    #region UpdateIsFollowingProperty

    [Fact]
    public void UpdateIsFollowingProperty_ReturnsIsFollowEqualsTrue_WhenViewModelUserIsInFollowingCollection()
    {
        // Arrange
        var sut = new UserNetworkHelpers();
        var viewModel = new NetworkUserViewModel() { UserName = "Test 2" };
        var followers = GetDynamicNetworkCollection(8);

        // Act
        var result = sut.UpdateIsFollowingProperty(viewModel.UserName, followers);

        // Assert
        var returnModel = Assert.IsType<bool>(result);
        //Assert.Equal(viewModel.UserName, returnModel.UserName);
        Assert.True(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(34)]
    public void UpdateIsFollowingProperty_ReturnsIsFollowEqualsFalse_WhenViewModelUserIsNotInFollowingCollection(int length)
    {
        // Arrange
        var sut = new UserNetworkHelpers();
        var viewModel = new NetworkUserViewModel() { UserName = "User Not In Following Collection" };
        var followers = GetDynamicNetworkCollection(length);

        // Act
        var result = sut.UpdateIsFollowingProperty(viewModel.UserName, followers);

        // Assert
        var returnModel = Assert.IsType<bool>(result);
        //Assert.Equal(viewModel.UserName, returnModel.UserName);
        Assert.False(result);
    }


    [Fact]
    public void UpdateIsFollowingProperty_ReturnsNullReferenceException_WhenFollowingCollectionIsNull()
    {
        // Arrange
        var sut = new UserNetworkHelpers();
        var viewModel = new NetworkUserViewModel() { UserName = "Test User" };

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => sut.UpdateIsFollowingProperty(viewModel.UserName, null));
        Assert.Equal("The followers collection is null (Parameter 'requestedUsersFollowers')", ex.Message);
    }

    [Fact]
    public void UpdateIsFollowingProperty_ReturnsNullReferenceException_WhenUsernameIsNull()
    {
        // Arrange
        var sut = new UserNetworkHelpers();

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => sut.UpdateIsFollowingProperty(null, new List<Network>()));
        Assert.Equal("The requestingUsername is null (Parameter 'requestingUsername')", ex.Message);
    }

    #endregion


    #region SetupFollowingCollection tests

    [Fact]
    public void SetupFollowingCollection_ReturnsArgumentNullException_WhenUserIsNull()
    {
        // Arrange
        var sut = new UserNetworkHelpers();

        ApplicationUser requestingUser = null;
        IEnumerable<FollowingViewModel> following = new List<FollowingViewModel>();

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => sut.SetupFollowingCollection(requestingUser, following));
        Assert.Equal("The requesting user is null (Parameter 'requestingUser')", ex.Message);
    }

    [Fact]
    public void SetupFollowingCollection_ReturnsArgumentNullException_WhenFollowingIsNull()
    {
        ApplicationUser requestingUser = new ApplicationUser();
        IEnumerable<FollowingViewModel> following = null;

        // Arrange
        var sut = new UserNetworkHelpers();


        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => sut.SetupFollowingCollection(requestingUser, following));
        Assert.Equal("The following collection is null (Parameter 'following')", ex.Message);
    }

    #endregion



    #region SetupFollowersCollection tests

    [Fact]
    public void SetupFollowersCollection_ReturnsArgumentNullException_WhenUserIsNull()
    {
        ApplicationUser requestingUser = null;
        IEnumerable<FollowerViewModel> followers = new List<FollowerViewModel>();

        // Arrange
        var sut = new UserNetworkHelpers();


        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => sut.SetupFollowersCollection(requestingUser, followers));
        Assert.Equal("The requesting user is null (Parameter 'requestingUser')", ex.Message);
    }

    [Fact]
    public void SetupFollowersCollection_ReturnsArgumentNullException_WhenFollowersIsNull()
    {
        ApplicationUser requestingUser = new ApplicationUser();
        IEnumerable<FollowerViewModel> followers = null;

        // Arrange
        var sut = new UserNetworkHelpers();


        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => sut.SetupFollowersCollection(requestingUser, followers));
        Assert.Equal("The followers collection is null (Parameter 'followers')", ex.Message);
    }


    #endregion


    private List<Network> GetDynamicNetworkCollection(int length)
    {
        var list = new List<Network>();

        for (int i = 0; i < length; i++)
        {
            list.Add(new Network()
            {
                Follower = new ApplicationUser { UserName = "Test " + (i + 1).ToString() },
                ApplicationUser = new ApplicationUser { UserName = "Test " + (i + 1).ToString() }
            });
        }

        return list;
    }
}
using Catalog.ApplicationLogic.CommentsCommands;
using Catalog.ApplicationLogic.CommentsQueries;
using Catalog.ApplicationLogic.Events;
using Catalog.ApplicationLogic.Events.EventHandlers;
using Catalog.ApplicationLogic.Infrastructure;
using Catalog.ApplicationLogic.LikeCommands;
using Catalog.ApplicationLogic.ObjectCommands;
using Catalog.ApplicationLogic.ObjectQueries;
using Catalog.ApplicationLogic.TagCommands;
using Catalog.ApplicationLogic.TypeQueries;
using Catalog.DataAccessLayer;
using CommonLibrary;
using EventBus;
using HostingHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace Catalog.ApplicationLogic
{
    public class AppLogicUnityConfig : IUnityConfigueration
    {
        public static IUnityContainer Container { get; private set; }

        public void ConfigUnity(IUnityContainer container)
        {
            Container = container;

            Container.RegisterType<CurrentUserCredentialsGetter>();
            Container.RegisterType<IUserDataManager, UserDataManager>();
            Container.RegisterType<UserDataGetter>();
            Container.RegisterType<IObjectAdder, ObjectAdder>();
            Container.RegisterType<IObjectDeleter, ObjectDeleter>();
            Container.RegisterType<IObjectQueryHelper, ObjectQueryHelper>();
            Container.RegisterType<IObjectsOrderedGetter, ObjectsOrderedGetter>();
            Container.RegisterType<IImageSaver, ImageSaver>();

            Container.RegisterType<IObjectGetter, ObjectGetter>();
            Container.RegisterType<IPhotoUrlConstructor, PhotoUrlConstructor>();

            Container.RegisterType<IObjectImpressionsManager, ObjectImpressionsManager>();
            Container.RegisterType<IObjectViewsManager, ObjectViewsManager>();
            Container.RegisterType<IObjectCommentAdder, ObjectCommentAdder>();
            Container.RegisterType<ICommentsGetter, CommentsGetter>();
            Container.RegisterType<IObjectCommentDeleter, ObjectCommentDeleter>();
            Container.RegisterType<TagsGetter>();
            Container.RegisterType<TagDeleter>();

            Container.RegisterType<IObjectDetailsGetter, ObjectDetailsGetter>();

            Container.RegisterType<ILikeAdder, LikeAdder>();
            Container.RegisterType<ILikeDeleter, LikeDeleter>();

            Container.RegisterType<TagAdder>();

            Container.RegisterType<LikeAdder>();
            Container.RegisterType<ObjectQueryHelper>();
            Container.RegisterType(typeof(OwnershipAuthorization<,>));

            Container.RegisterType<IIntegrationEventHandler<NewRegistrationIntegrationEvent>,NewRegistrationIntegrationEventHandler>();
            new DalUnityConfig().ConfigUnity(container);
        }
    }
}

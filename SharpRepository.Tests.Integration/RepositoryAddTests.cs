using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using NUnit.Framework;
using SharpRepository.Repository;
using SharpRepository.Tests.Integration.TestAttributes;
using SharpRepository.Tests.Integration.TestObjects;
using Should;

namespace SharpRepository.Tests.Integration
{
    [TestFixture]
    public class RepositoryAddTests : TestBase
    {
        [ExecuteForAllRepositories]
        public void Add_Should_Save_And_Assigned_New_Id(IRepository<Contact, string> repository)
        {
            var contact = new Contact { Name = "Test User" };
            repository.Add(contact);
            contact.ContactId.ShouldNotBeEmpty();
        }
        
        [ExecuteForAllRepositories]
        public void Add_Should_Result_In_Proper_Total_Items(IRepository<Contact, string> repository)
        {
            repository.Add(new Contact { Name = "Test User" });
            
            var result = repository.GetAll();
            result.Count().ShouldEqual(1);
        }

        [ExecuteForAllRepositories]
        public void Add_InBatchMode_Should_Delay_The_Action(IRepository<Contact, string> repository)
        {
            using (var batch = repository.BeginBatch())
            {
                batch.Add(new Contact { Name = "Test User 1" });

                repository.GetAll().Count().ShouldEqual(0); // shouldn't have really been added yet

                batch.Add(new Contact { Name = "Test User 2" });

                repository.GetAll().Count().ShouldEqual(0); // shouldn't have really been added yet

                batch.Commit();
            }

            repository.GetAll().Count().ShouldEqual(2);
        }

        [ExecuteForAllRepositories]
        public void Add_Should_Save_And_Assigned_New_Ids_To_Multiple(IRepository<Contact, string> repository)
        {
            IList<Contact> contacts = new List<Contact>
                                            {
                                                new Contact { Name = "Contact 1"},
                                                new Contact { Name = "Contact 2"},
                                                new Contact { Name = "Contact 3"},
                                        };

            repository.Add(contacts);
            contacts.First().ContactId.ShouldNotBeEmpty();
            contacts.Last().ContactId.ShouldNotBeEmpty();
            contacts.First().ShouldNotBeSameAs(contacts.Last().ContactId);
            
            var added = repository.GetAll();
            added.Count().ShouldEqual(3);
        }

        [ExecuteForAllRepositories]
        public void Using_TransactionScope_WIth_No_Commit_Should_Not_Add(IRepository<Contact, string> repository)
        {
            // ref: http://www.codeproject.com/Articles/31270/A-Transactional-Repository-Implementation-in-NET
            using (var trans = new TransactionScope())
            {
                repository.Add(new Contact {Name = "Contact 1"});

                //trans.Complete();
            }

            repository.GetAll().Count().ShouldEqual(0);
        }
    }
}
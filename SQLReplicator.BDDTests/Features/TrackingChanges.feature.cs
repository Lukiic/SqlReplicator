﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by Reqnroll (https://www.reqnroll.net/).
//      Reqnroll Version:2.0.0.0
//      Reqnroll Generator Version:2.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace SQLReplicator.BDDTests.Features
{
    using Reqnroll;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Reqnroll", "2.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class TrackingChangesFeature : object, Xunit.IClassFixture<TrackingChangesFeature.FixtureData>, Xunit.IAsyncLifetime
    {
        
        private global::Reqnroll.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
        private static global::Reqnroll.FeatureInfo featureInfo = new global::Reqnroll.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "TrackingChanges", null, global::Reqnroll.ProgrammingLanguage.CSharp, featureTags);
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "TrackingChanges.feature"
#line hidden
        
        public TrackingChangesFeature(TrackingChangesFeature.FixtureData fixtureData, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
        }
        
        public static async System.Threading.Tasks.Task FeatureSetupAsync()
        {
        }
        
        public static async System.Threading.Tasks.Task FeatureTearDownAsync()
        {
        }
        
        public async System.Threading.Tasks.Task TestInitializeAsync()
        {
            testRunner = global::Reqnroll.TestRunnerManager.GetTestRunnerForAssembly(featureHint: featureInfo);
            if (((testRunner.FeatureContext != null) 
                        && (testRunner.FeatureContext.FeatureInfo.Equals(featureInfo) == false)))
            {
                await testRunner.OnFeatureEndAsync();
            }
            if ((testRunner.FeatureContext == null))
            {
                await testRunner.OnFeatureStartAsync(featureInfo);
            }
        }
        
        public async System.Threading.Tasks.Task TestTearDownAsync()
        {
            await testRunner.OnScenarioEndAsync();
            global::Reqnroll.TestRunnerManager.ReleaseTestRunner(testRunner);
        }
        
        public void ScenarioInitialize(global::Reqnroll.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public async System.Threading.Tasks.Task ScenarioStartAsync()
        {
            await testRunner.OnScenarioStartAsync();
        }
        
        public async System.Threading.Tasks.Task ScenarioCleanupAsync()
        {
            await testRunner.CollectScenarioErrorsAsync();
        }
        
        async System.Threading.Tasks.Task Xunit.IAsyncLifetime.InitializeAsync()
        {
            await this.TestInitializeAsync();
        }
        
        async System.Threading.Tasks.Task Xunit.IAsyncLifetime.DisposeAsync()
        {
            await this.TestTearDownAsync();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Insertion in Orders table is tracked in OrdersChanges table")]
        [Xunit.TraitAttribute("FeatureTitle", "TrackingChanges")]
        [Xunit.TraitAttribute("Description", "Insertion in Orders table is tracked in OrdersChanges table")]
        public async System.Threading.Tasks.Task InsertionInOrdersTableIsTrackedInOrdersChangesTable()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("Insertion in Orders table is tracked in OrdersChanges table", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 3
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
                global::Reqnroll.Table table20 = new global::Reqnroll.Table(new string[] {
                            "AttributeName"});
                table20.AddRow(new string[] {
                            "OrderID"});
                table20.AddRow(new string[] {
                            "ProductID"});
#line 4
 await testRunner.GivenAsync("database \"SourceDB\" has a trigger and an empty change tracking table for table \"O" +
                        "rders\" with key attributes:", ((string)(null)), table20, "Given ");
#line hidden
                global::Reqnroll.Table table21 = new global::Reqnroll.Table(new string[] {
                            "OrderID",
                            "ProductID",
                            "CustomerName",
                            "Quantity"});
                table21.AddRow(new string[] {
                            "998",
                            "924124",
                            "Tom Hanks",
                            "12"});
#line 8
 await testRunner.WhenAsync("I insert new row in database \"SourceDB\" table \"Orders\" with values:", ((string)(null)), table21, "When ");
#line hidden
                global::Reqnroll.Table table22 = new global::Reqnroll.Table(new string[] {
                            "OrderID",
                            "ProductID",
                            "Operation"});
                table22.AddRow(new string[] {
                            "998",
                            "924124",
                            "I"});
#line 11
 await testRunner.ThenAsync("the table \"OrdersChanges\" in database \"SourceDB\" should have row with values:", ((string)(null)), table22, "Then ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Deletion in Orders table is tracked in OrdersChanges table")]
        [Xunit.TraitAttribute("FeatureTitle", "TrackingChanges")]
        [Xunit.TraitAttribute("Description", "Deletion in Orders table is tracked in OrdersChanges table")]
        public async System.Threading.Tasks.Task DeletionInOrdersTableIsTrackedInOrdersChangesTable()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("Deletion in Orders table is tracked in OrdersChanges table", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 15
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
                global::Reqnroll.Table table23 = new global::Reqnroll.Table(new string[] {
                            "AttributeName"});
                table23.AddRow(new string[] {
                            "OrderID"});
                table23.AddRow(new string[] {
                            "ProductID"});
#line 16
 await testRunner.GivenAsync("database \"SourceDB\" has a trigger and an empty change tracking table for table \"O" +
                        "rders\" with key attributes:", ((string)(null)), table23, "Given ");
#line hidden
                global::Reqnroll.Table table24 = new global::Reqnroll.Table(new string[] {
                            "OrderID",
                            "ProductID",
                            "CustomerName",
                            "Quantity"});
                table24.AddRow(new string[] {
                            "999",
                            "924125",
                            "Tom Hanks",
                            "13"});
#line 20
 await testRunner.WhenAsync("I delete existing row in database \"SourceDB\" table \"Orders\" with values:", ((string)(null)), table24, "When ");
#line hidden
                global::Reqnroll.Table table25 = new global::Reqnroll.Table(new string[] {
                            "OrderID",
                            "ProductID",
                            "Operation"});
                table25.AddRow(new string[] {
                            "999",
                            "924125",
                            "D"});
#line 23
 await testRunner.ThenAsync("the table \"OrdersChanges\" in database \"SourceDB\" should have row with values:", ((string)(null)), table25, "Then ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Update in Orders table is tracked in OrdersChanges table")]
        [Xunit.TraitAttribute("FeatureTitle", "TrackingChanges")]
        [Xunit.TraitAttribute("Description", "Update in Orders table is tracked in OrdersChanges table")]
        public async System.Threading.Tasks.Task UpdateInOrdersTableIsTrackedInOrdersChangesTable()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("Update in Orders table is tracked in OrdersChanges table", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 27
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
                global::Reqnroll.Table table26 = new global::Reqnroll.Table(new string[] {
                            "AttributeName"});
                table26.AddRow(new string[] {
                            "OrderID"});
                table26.AddRow(new string[] {
                            "ProductID"});
#line 28
 await testRunner.GivenAsync("database \"SourceDB\" has a trigger and an empty change tracking table for table \"O" +
                        "rders\" with key attributes:", ((string)(null)), table26, "Given ");
#line hidden
                global::Reqnroll.Table table27 = new global::Reqnroll.Table(new string[] {
                            "OrderID",
                            "ProductID",
                            "CustomerName",
                            "Quantity"});
                table27.AddRow(new string[] {
                            "1",
                            "101",
                            "Alice Johnson",
                            "2"});
#line 32
 await testRunner.WhenAsync("I update existing row in database \"SourceDB\" table \"Orders\" with values:", ((string)(null)), table27, "When ");
#line hidden
                global::Reqnroll.Table table28 = new global::Reqnroll.Table(new string[] {
                            "OrderID",
                            "ProductID",
                            "Operation"});
                table28.AddRow(new string[] {
                            "1",
                            "101",
                            "U"});
#line 35
 await testRunner.ThenAsync("the table \"OrdersChanges\" in database \"SourceDB\" should have row with values:", ((string)(null)), table28, "Then ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Reqnroll", "2.0.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : object, Xunit.IAsyncLifetime
        {
            
            async System.Threading.Tasks.Task Xunit.IAsyncLifetime.InitializeAsync()
            {
                await TrackingChangesFeature.FeatureSetupAsync();
            }
            
            async System.Threading.Tasks.Task Xunit.IAsyncLifetime.DisposeAsync()
            {
                await TrackingChangesFeature.FeatureTearDownAsync();
            }
        }
    }
}
#pragma warning restore
#endregion

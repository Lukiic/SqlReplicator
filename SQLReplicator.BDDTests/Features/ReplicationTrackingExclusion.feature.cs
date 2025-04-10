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
    public partial class ReplicationTrackingExclusionFeature : object, Xunit.IClassFixture<ReplicationTrackingExclusionFeature.FixtureData>, Xunit.IAsyncLifetime
    {
        
        private global::Reqnroll.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
        private static global::Reqnroll.FeatureInfo featureInfo = new global::Reqnroll.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "ReplicationTrackingExclusion", null, global::Reqnroll.ProgrammingLanguage.CSharp, featureTags);
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "ReplicationTrackingExclusion.feature"
#line hidden
        
        public ReplicationTrackingExclusionFeature(ReplicationTrackingExclusionFeature.FixtureData fixtureData, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
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
        
        [Xunit.SkippableFactAttribute(DisplayName="Changes replicated to the Orders table of destination server are not tracked in t" +
            "he change tracking table")]
        [Xunit.TraitAttribute("FeatureTitle", "ReplicationTrackingExclusion")]
        [Xunit.TraitAttribute("Description", "Changes replicated to the Orders table of destination server are not tracked in t" +
            "he change tracking table")]
        public async System.Threading.Tasks.Task ChangesReplicatedToTheOrdersTableOfDestinationServerAreNotTrackedInTheChangeTrackingTable()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("Changes replicated to the Orders table of destination server are not tracked in t" +
                    "he change tracking table", null, tagsOfScenario, argumentsOfScenario, featureTags);
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
                global::Reqnroll.Table table16 = new global::Reqnroll.Table(new string[] {
                            "AttributeName"});
                table16.AddRow(new string[] {
                            "OrderID"});
                table16.AddRow(new string[] {
                            "ProductID"});
#line 4
 await testRunner.GivenAsync("database \"SourceDB\" has a trigger and an empty change tracking table for table \"O" +
                        "rders\" with key attributes:", ((string)(null)), table16, "Given ");
#line hidden
                global::Reqnroll.Table table17 = new global::Reqnroll.Table(new string[] {
                            "AttributeName"});
                table17.AddRow(new string[] {
                            "OrderID"});
                table17.AddRow(new string[] {
                            "ProductID"});
#line 8
 await testRunner.AndAsync("database \"DestinationDB\" has a trigger and an empty change tracking table for tab" +
                        "le \"Orders\" with key attributes:", ((string)(null)), table17, "And ");
#line hidden
                global::Reqnroll.Table table18 = new global::Reqnroll.Table(new string[] {
                            "OrderID",
                            "ProductID",
                            "CustomerName",
                            "Quantity"});
                table18.AddRow(new string[] {
                            "998",
                            "924124",
                            "Tom Hanks",
                            "12"});
#line 12
 await testRunner.WhenAsync("I insert new row in database \"SourceDB\" table \"Orders\" with values:", ((string)(null)), table18, "When ");
#line hidden
                global::Reqnroll.Table table19 = new global::Reqnroll.Table(new string[] {
                            "AttributeName"});
                table19.AddRow(new string[] {
                            "OrderID"});
                table19.AddRow(new string[] {
                            "ProductID"});
#line 15
 await testRunner.AndAsync("I run service for generating commands on database \"SourceDB\" for table \"Orders\" w" +
                        "ith key attributes:", ((string)(null)), table19, "And ");
#line hidden
#line 19
 await testRunner.AndAsync("I run service for executing generated commands on database \"DestinationDB\"", ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
#line 20
 await testRunner.ThenAsync("table named \"OrdersChanges\" in database \"DestinationDB\" should be empty", ((string)(null)), ((global::Reqnroll.Table)(null)), "Then ");
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
                await ReplicationTrackingExclusionFeature.FeatureSetupAsync();
            }
            
            async System.Threading.Tasks.Task Xunit.IAsyncLifetime.DisposeAsync()
            {
                await ReplicationTrackingExclusionFeature.FeatureTearDownAsync();
            }
        }
    }
}
#pragma warning restore
#endregion

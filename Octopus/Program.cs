using System;
using System.Windows.Forms;

namespace Octopus
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //var dummyData = new List<GroupedConditionsDto>
            //{
            //    new GroupedConditionsDto
            //    {
            //        GroupId = "G1",
            //        GroupCount = 2,
            //        Conditions = new List<ConditionDto>
            //        {
            //            new ConditionDto { Condition = "Condition 1" },
            //            new ConditionDto { Condition = "Condition 2" },
            //        }
            //    },
            //    new GroupedConditionsDto
            //    {
            //        GroupId = "G2",
            //        GroupCount = 2,
            //        Conditions = new List<ConditionDto>
            //        {
            //            new ConditionDto { Condition = "Condition 3" },
            //            new ConditionDto { Condition = "Condition 4" }
            //        }
            //    },
            //    new GroupedConditionsDto
            //    {
            //        GroupId = "G3",
            //        GroupCount = 4,
            //        Conditions = new List<ConditionDto>
            //        {
            //            new ConditionDto { Condition = "Condition 5" },
            //            new ConditionDto { Condition = "Condition 6" },
            //            new ConditionDto { Condition = "Condition 7" },
            //            new ConditionDto { Condition = "Condition 8" }
            //        }
            //    },new GroupedConditionsDto
            //    {
            //        GroupId = "G4",
            //        GroupCount = 2,
            //        Conditions = new List<ConditionDto>
            //        {
            //            new ConditionDto { Condition = "Condition 9" },
            //            new ConditionDto { Condition = "Condition 10" }
            //        }
            //    },new GroupedConditionsDto
            //    {
            //        GroupId = "G5",
            //        GroupCount = 2,
            //        Conditions = new List<ConditionDto>
            //        {
            //            new ConditionDto { Condition = "Condition 11" },
            //            new ConditionDto { Condition = "Condition 12" }
            //        }
            //    },new GroupedConditionsDto
            //    {
            //        GroupId = "G6",
            //        GroupCount = 6,
            //        Conditions = new List<ConditionDto>
            //        {
            //            new ConditionDto { Condition = "Condition 13" },
            //            new ConditionDto { Condition = "Condition 14" },
            //            new ConditionDto { Condition = "Condition 15" },
            //            new ConditionDto { Condition = "Condition 16" },
            //            new ConditionDto { Condition = "Condition 17" },
            //            new ConditionDto { Condition = "Condition 18" },
            //        }
            //    }
            //    ,new GroupedConditionsDto
            //    {
            //        GroupId = "G7",
            //        GroupCount = 1,
            //        Conditions = new List<ConditionDto>
            //        {
            //            new ConditionDto { Condition = "Condition 19" }
            //        }
            //    },
            //    // Add more scenarios as needed
            //};
            // JSON data as a string
            //string jsonData = @"
            // {
            //   ""response"": ""SUCCESS"",
            //   ""statusCode"": 200,
            //   ""message"": ""SUCCESS"",
            //   ""requestId"": ""837d5726-5463-43a9-b094-010c03de7f5b"",
            //   ""data"": [
            //     {
            //       ""groupId"": ""G1"",
            //       ""groupCount"": 2,
            //       ""conditions"": [
            //         {
            //           ""condition"": ""Allergic Sinusitis""
            //         },
            //         {
            //           ""condition"": ""Allergic Rhinitis""
            //         }
            //       ]
            //     },
            //     {
            //       ""groupId"": ""G1"",
            //       ""groupCount"": 2,
            //       ""conditions"": [
            //         {
            //           ""condition"": ""Cellulitis Of Breast""
            //         },
            //         {
            //           ""condition"": ""Mastitis Without Abscess""
            //         }
            //       ]
            //     },
            //     {
            //       ""groupId"": ""G1"",
            //       ""groupCount"": 2,
            //       ""conditions"": [
            //         {
            //           ""condition"": ""Acute Nasopharyngitis""
            //         },
            //         {
            //           ""condition"": ""Acute Rhinitis""
            //         }
            //       ]
            //     },
            //     {
            //       ""groupId"": ""G1"",
            //       ""groupCount"": 2,
            //       ""conditions"": [
            //         {
            //           ""condition"": ""Acute Pharyngitis""
            //         }
            //       ]
            //     },
            //     {
            //       ""groupId"": ""G1"",
            //       ""groupCount"": 2,
            //       ""conditions"": [
            //         {
            //           ""condition"": ""Paratyphoid Fever""
            //         }
            //       ]
            //     },
            //     {
            //       ""groupId"": ""G1"",
            //       ""groupCount"": 2,
            //       ""conditions"": [
            //         {
            //           ""condition"": ""Malignant (Primary) Neoplasm""
            //         }
            //       ]
            //     },
            //     {
            //       ""groupId"": ""G1"",
            //       ""groupCount"": 2,
            //       ""conditions"": [
            //         {
            //           ""condition"": ""Periodic Fever Syndromes""
            //         }
            //       ]
            //     },
            //     {
            //       ""groupId"": ""G1"",
            //       ""groupCount"": 2,
            //       ""conditions"": [
            //         {
            //           ""condition"": ""Fever""
            //         },
            //         {
            //           ""condition"": ""Chills With Fever""
            //         }
            //       ]
            //     }
            //   ]
            // }";
            //// //// Parse the JSON string into a dynamic object
            //dynamic dynamicData = JsonConvert.DeserializeObject(jsonData);

            //// Access the "data" property
            //JArray dataArray = dynamicData.data;

            //// //Convert the "data" array back to a list of GroupedConditionsDto
            //var deserializedData = dataArray.ToObject<List<GroupedConditionsDto>>();
            //Your JSON data
            //string jsonData = @"
            //{
            //    ""response"": ""SUCCESS"",
            //    ""statusCode"": 200,
            //    ""message"": ""SUCCESS"",
            //    ""requestId"": ""ff20e3af-0974-4a2f-ba2c-e02c64e092d3"",
            //    ""data"": [
            //        {
            //            ""groupId"": ""G1"",
            //            ""groupCount"": 2,
            //            ""conditions"": [
            //                {
            //                    ""condition"": ""Type 2 Diabetes Mellitus With Diabetic Nephropathy""
            //                }
            //            ]
            //        },
            //        {
            //            ""groupId"": ""G1"",
            //            ""groupCount"": 2,
            //            ""conditions"": [
            //                {
            //                    ""condition"": ""Cavernitis""
            //                },
            //                {
            //                    ""condition"": ""Acute Lymphangitis Of Penis""
            //                }
            //            ]
            //        },
            //        {
            //            ""groupId"": ""G1"",
            //            ""groupCount"": 2,
            //            ""conditions"": [
            //                {
            //                    ""condition"": ""Allergic Sinusitis""
            //                },
            //                {
            //                    ""condition"": ""Allergic Rhinitis""
            //                }
            //            ]
            //        },
            //        {
            //            ""groupId"": ""G1"",
            //            ""groupCount"": 2,
            //            ""conditions"": [
            //                {
            //                    ""condition"": ""Acute Nasopharyngitis""
            //                }
            //            ]
            //        },
            //        {
            //            ""groupId"": ""G1"",
            //            ""groupCount"": 2,
            //            ""conditions"": [
            //                {
            //                    ""condition"": ""Paratyphoid Fever""
            //                }
            //            ]
            //        },
            //        {
            //            ""groupId"": ""G1"",
            //            ""groupCount"": 2,
            //            ""conditions"": [
            //                {
            //                    ""condition"": ""Malignant (Primary) Neoplasm""
            //                }
            //            ]
            //        },
            //        {
            //            ""groupId"": ""G1"",
            //            ""groupCount"": 2,
            //            ""conditions"": [
            //                {
            //                    ""condition"": ""Periodic Fever Syndromes""
            //                }
            //            ]
            //        },
            //        {
            //            ""groupId"": ""G1"",
            //            ""groupCount"": 2,
            //            ""conditions"": [
            //                {
            //                    ""condition"": ""Varicose Veins Of Lower Extremity With Ulcer""
            //                }
            //            ]
            //        },
            //        {
            //            ""groupId"": ""G3"",
            //            ""groupCount"": 2,
            //            ""conditions"": [
            //                {
            //                    ""condition"": ""Fever""
            //                },{
            //                    ""condition"": ""New Condition 1""
            //                },
            //                {
            //                    ""condition"": ""New Condition 2""
            //                },
            //                {
            //                    ""condition"": ""New Condition 3""
            //                },
            //                {
            //                    ""condition"": ""New Condition 4""
            //                },
            //                {
            //                    ""condition"": ""New Condition 5""
            //                }
            //            ]
            //        },
            //        {
            //            ""groupId"": ""G2"",
            //            ""groupCount"": 2,
            //            ""conditions"": [
            //                {
            //                    ""condition"": ""Diabetic Nephrotic Syndrome""
            //                },
            //                {
            //                    ""condition"": ""New Condition 1""
            //                },
            //                {
            //                    ""condition"": ""New Condition 2""
            //                },
            //                {
            //                    ""condition"": ""New Condition 3""
            //                },
            //                {
            //                    ""condition"": ""New Condition 4""
            //                },
            //                {
            //                    ""condition"": ""New Condition 5""
            //                },
            //                {
            //                    ""condition"": ""New Condition 5""
            //                },
            //                {
            //                    ""condition"": ""New Condition 5""
            //                }
            //            ]
            //        }
            //    ]
            //}";


            //dynamic dynamicData = JsonConvert.DeserializeObject(jsonData);
            //JArray dataArray = dynamicData.data;

            //var groupedConditionsList = new List<GroupedConditionsDto>();

            //foreach (var group in dataArray.GroupBy(item => (string)item["groupId"]))
            //{
            //    var conditions = group.SelectMany(item => item["conditions"].ToObject<List<ConditionDto>>()).ToList();

            //    var groupedConditions = new GroupedConditionsDto
            //    {
            //        GroupId = group.Key,
            //        GroupCount = conditions.Count,
            //        Conditions = conditions
            //    };

            //    groupedConditionsList.Add(groupedConditions);
            //}


            //var test = JsonConvert.SerializeObject(groupedConditionsList);

            //// Print the restructured data
            //foreach (var group in groupedConditionsList)
            //{
            //    Console.WriteLine($"GroupId: {group.GroupId}, GroupCount: {group.GroupCount}");
            //    foreach (var condition in group.Conditions)
            //    {
            //        Console.WriteLine($"  Condition: {condition.Condition}");
            //    }
            //    Console.WriteLine();
            //}


            // Pass this dummy data to your ICDSuggestions constructor
            //ICDSuggestions suggestions = new ICDSuggestions(dummyData, new Dtos.PatientConditionsDtos.PatientConditionsDto());
            //Create an instance of BaseForm

            //            List<CPTSuggestionsResponseDto> dummyData1 = new List<CPTSuggestionsResponseDto>
            //{
            //    new CPTSuggestionsResponseDto
            //    {
            //        serviceType = "ServiceType1",
            //        items = new List<items>
            //        {
            //            new items
            //            {
            //                code = "Code1",
            //                description = "Description1",
            //                type = "Type1",
            //                custHISCPTId = "CPTId1",
            //                custHISServiceType = "ServiceType1"
            //            },
            //            new items
            //            {
            //                code = "Code2",
            //                description = "Description2",
            //                type = "Type2",
            //                custHISCPTId = "CPTId2",
            //                custHISServiceType = "ServiceType2"
            //            }
            //            // Add more items as needed
            //        }
            //    },
            //    new CPTSuggestionsResponseDto
            //    {
            //        serviceType = "ServiceType2",
            //        items = new List<items>
            //        {
            //            new items
            //            {
            //                code = "Code3",
            //                description = "Description3",
            //                type = "Type3",
            //                custHISCPTId = "CPTId3",
            //                custHISServiceType = "ServiceType3"
            //            },
            //            new items
            //            {
            //                code = "Code4",
            //                description = "Description4",
            //                type = "Type4",
            //                custHISCPTId = "CPTId4",
            //                custHISServiceType = "ServiceType4"
            //            },
            //             new items
            //            {
            //                code = "Code4",
            //                description = "Description4",
            //                type = "Type4",
            //                custHISCPTId = "CPTId4",
            //                custHISServiceType = "ServiceType4"
            //            },
            //             new items
            //            {
            //                code = "Code4",
            //                description = "Description4",
            //                type = "Type4",
            //                custHISCPTId = "CPTId4",
            //                custHISServiceType = "ServiceType4"
            //            },
            //             new items
            //            {
            //                code = "Code4",
            //                description = "Description4",
            //                type = "Type4",
            //                custHISCPTId = "CPTId4",
            //                custHISServiceType = "ServiceType4"
            //            },
            //             new items
            //            {
            //                code = "Code4",
            //                description = "Description4",
            //                type = "Type4",
            //                custHISCPTId = "CPTId4",
            //                custHISServiceType = "ServiceType4"
            //            },
            //             new items
            //            {
            //                code = "Code4",
            //                description = "Description4",
            //                type = "Type4",
            //                custHISCPTId = "CPTId4",
            //                custHISServiceType = "ServiceType4"
            //            },
            //             new items
            //            {
            //                code = "Code4",
            //                description = "Description4",
            //                type = "Type4",
            //                custHISCPTId = "CPTId4",
            //                custHISServiceType = "ServiceType4"
            //            },
            //             new items
            //            {
            //                code = "Code4",
            //                description = "Description4",
            //                type = "Type4",
            //                custHISCPTId = "CPTId4",
            //                custHISServiceType = "ServiceType4"
            //            },
            //             new items
            //            {
            //                code = "Code4",
            //                description = "Description4",
            //                type = "Type4",
            //                custHISCPTId = "CPTId4",
            //                custHISServiceType = "ServiceType4"
            //            }
            //            // Add more items as needed
            //        }
            //    }
            //    // Add more service types as needed
            //};
            //List<RulesValidationResult> dummyData = new List<RulesValidationResult>
            //{
            //    new RulesValidationResult
            //    {
            //        id = 1,
            //        ruleName = "Rule1",
            //        ruleCode = "R001",
            //        errorMessage = "Error in Rule 1",
            //        longErrorMessage = "Detailed error message for Rule 1",
            //        priority = "High",
            //        solution = "Fix for Rule 1",
            //        action = "Take action for Rule 1",
            //        item = "Item1",
            //        itemDescription = "Description for Item1",
            //        ICDIsSigns = 1,
            //        HISCusId = "CusId1",
            //        Section = "SectionA",
            //        Qty = "1",
            //        ReplaceWith = "Replacement1",
            //        replaceCodes = new List<string> { "Code1", "Code2" },
            //        ReplaceWithHISCusId = "ReplaceCusId1",
            //        CustHISServiceType = "ServiceTypeA",
            //        ReplaceCusHISServiceType = "ReplaceServiceTypeA",
            //        CustHISDescription = "Description for ServiceTypeA",
            //        ReplaceCusHISDescription = "ReplaceDescriptionA",
            //        icdCode = new List<string> { "ICDCode1", "ICDCode2" },
            //        codes = new List<string> { "CodeA", "CodeB" },
            //        ruleRank = 1,
            //        ItemRemoval = new List<ItemRemoval>
            //        {
            //            new ItemRemoval { Code = "RemovalCode1", Description = "RemovalDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemRemoval { Code = "RemovalCode2", Description = "RemovalDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        },
            //        ItemAddition = new List<ItemAddition>
            //        {
            //            new ItemAddition { Code = "AdditionCode1", Description = "AdditionDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemAddition { Code = "AdditionCode2", Description = "AdditionDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        }
            //    },
            //     new RulesValidationResult
            //    {
            //        id = 1,
            //        ruleName = "Rule1",
            //        ruleCode = "R001",
            //        errorMessage = "Error in Rule 1",
            //        longErrorMessage = "Detailed error message for Rule 1",
            //        priority = "High",
            //        solution = "Fix for Rule 1",
            //        action = "Take action for Rule 1",
            //        item = "Item1",
            //        itemDescription = "Description for Item1",
            //        ICDIsSigns = 1,
            //        HISCusId = "CusId1",
            //        Section = "SectionA",
            //        Qty = "1",
            //        ReplaceWith = "Replacement1",
            //        replaceCodes = new List<string> { "Code1", "Code2" },
            //        ReplaceWithHISCusId = "ReplaceCusId1",
            //        CustHISServiceType = "ServiceTypeA",
            //        ReplaceCusHISServiceType = "ReplaceServiceTypeA",
            //        CustHISDescription = "Description for ServiceTypeA",
            //        ReplaceCusHISDescription = "ReplaceDescriptionA",
            //        icdCode = new List<string> { "ICDCode1", "ICDCode2" },
            //        codes = new List<string> { "CodeA", "CodeB" },
            //        ruleRank = 1,
            //        ItemRemoval = new List<ItemRemoval>
            //        {
            //            new ItemRemoval { Code = "RemovalCode1", Description = "RemovalDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemRemoval { Code = "RemovalCode2", Description = "RemovalDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        },
            //        ItemAddition = new List<ItemAddition>
            //        {
            //            new ItemAddition { Code = "AdditionCode1", Description = "AdditionDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemAddition { Code = "AdditionCode2", Description = "AdditionDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        }
            //    }, new RulesValidationResult
            //    {
            //        id = 1,
            //        ruleName = "Rule1",
            //        ruleCode = "R001",
            //        errorMessage = "Error in Rule 1",
            //        longErrorMessage = "Detailed error message for Rule 1",
            //        priority = "High",
            //        solution = "Fix for Rule 1",
            //        action = "Take action for Rule 1",
            //        item = "Item1",
            //        itemDescription = "Description for Item1",
            //        ICDIsSigns = 1,
            //        HISCusId = "CusId1",
            //        Section = "SectionA",
            //        Qty = "1",
            //        ReplaceWith = "Replacement1",
            //        replaceCodes = new List<string> { "Code1", "Code2" },
            //        ReplaceWithHISCusId = "ReplaceCusId1",
            //        CustHISServiceType = "ServiceTypeA",
            //        ReplaceCusHISServiceType = "ReplaceServiceTypeA",
            //        CustHISDescription = "Description for ServiceTypeA",
            //        ReplaceCusHISDescription = "ReplaceDescriptionA",
            //        icdCode = new List<string> { "ICDCode1", "ICDCode2" },
            //        codes = new List<string> { "CodeA", "CodeB" },
            //        ruleRank = 1,
            //        ItemRemoval = new List<ItemRemoval>
            //        {
            //            new ItemRemoval { Code = "RemovalCode1", Description = "RemovalDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemRemoval { Code = "RemovalCode2", Description = "RemovalDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        },
            //        ItemAddition = new List<ItemAddition>
            //        {
            //            new ItemAddition { Code = "AdditionCode1", Description = "AdditionDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemAddition { Code = "AdditionCode2", Description = "AdditionDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        }
            //    }, new RulesValidationResult
            //    {
            //        id = 1,
            //        ruleName = "Rule1",
            //        ruleCode = "R001",
            //        errorMessage = "Error in Rule 1",
            //        longErrorMessage = "Detailed error message for Rule 1",
            //        priority = "High",
            //        solution = "Fix for Rule 1",
            //        action = "Take action for Rule 1",
            //        item = "Item1",
            //        itemDescription = "Description for Item1",
            //        ICDIsSigns = 1,
            //        HISCusId = "CusId1",
            //        Section = "SectionA",
            //        Qty = "1",
            //        ReplaceWith = "Replacement1",
            //        replaceCodes = new List<string> { "Code1", "Code2" },
            //        ReplaceWithHISCusId = "ReplaceCusId1",
            //        CustHISServiceType = "ServiceTypeA",
            //        ReplaceCusHISServiceType = "ReplaceServiceTypeA",
            //        CustHISDescription = "Description for ServiceTypeA",
            //        ReplaceCusHISDescription = "ReplaceDescriptionA",
            //        icdCode = new List<string> { "ICDCode1", "ICDCode2" },
            //        codes = new List<string> { "CodeA", "CodeB" },
            //        ruleRank = 1,
            //        ItemRemoval = new List<ItemRemoval>
            //        {
            //            new ItemRemoval { Code = "RemovalCode1", Description = "RemovalDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemRemoval { Code = "RemovalCode2", Description = "RemovalDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        },
            //        ItemAddition = new List<ItemAddition>
            //        {
            //            new ItemAddition { Code = "AdditionCode1", Description = "AdditionDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemAddition { Code = "AdditionCode2", Description = "AdditionDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        }
            //    }, new RulesValidationResult
            //    {
            //        id = 1,
            //        ruleName = "Rule1",
            //        ruleCode = "R001",
            //        errorMessage = "Error in Rule 1",
            //        longErrorMessage = "Detailed error message for Rule 1",
            //        priority = "High",
            //        solution = "Fix for Rule 1",
            //        action = "Take action for Rule 1",
            //        item = "Item1",
            //        itemDescription = "Description for Item1",
            //        ICDIsSigns = 1,
            //        HISCusId = "CusId1",
            //        Section = "SectionA",
            //        Qty = "1",
            //        ReplaceWith = "Replacement1",
            //        replaceCodes = new List<string> { "Code1", "Code2" },
            //        ReplaceWithHISCusId = "ReplaceCusId1",
            //        CustHISServiceType = "ServiceTypeA",
            //        ReplaceCusHISServiceType = "ReplaceServiceTypeA",
            //        CustHISDescription = "Description for ServiceTypeA",
            //        ReplaceCusHISDescription = "ReplaceDescriptionA",
            //        icdCode = new List<string> { "ICDCode1", "ICDCode2" },
            //        codes = new List<string> { "CodeA", "CodeB" },
            //        ruleRank = 1,
            //        ItemRemoval = new List<ItemRemoval>
            //        {
            //            new ItemRemoval { Code = "RemovalCode1", Description = "RemovalDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemRemoval { Code = "RemovalCode2", Description = "RemovalDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        },
            //        ItemAddition = new List<ItemAddition>
            //        {
            //            new ItemAddition { Code = "AdditionCode1", Description = "AdditionDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemAddition { Code = "AdditionCode2", Description = "AdditionDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        }
            //    }, new RulesValidationResult
            //    {
            //        id = 1,
            //        ruleName = "Rule1",
            //        ruleCode = "R001",
            //        errorMessage = "Error in Rule 1",
            //        longErrorMessage = "Detailed error message for Rule 1",
            //        priority = "High",
            //        solution = "Fix for Rule 1",
            //        action = "Take action for Rule 1",
            //        item = "Item1",
            //        itemDescription = "Description for Item1",
            //        ICDIsSigns = 1,
            //        HISCusId = "CusId1",
            //        Section = "SectionA",
            //        Qty = "1",
            //        ReplaceWith = "Replacement1",
            //        replaceCodes = new List<string> { "Code1", "Code2" },
            //        ReplaceWithHISCusId = "ReplaceCusId1",
            //        CustHISServiceType = "ServiceTypeA",
            //        ReplaceCusHISServiceType = "ReplaceServiceTypeA",
            //        CustHISDescription = "Description for ServiceTypeA",
            //        ReplaceCusHISDescription = "ReplaceDescriptionA",
            //        icdCode = new List<string> { "ICDCode1", "ICDCode2" },
            //        codes = new List<string> { "CodeA", "CodeB" },
            //        ruleRank = 1,
            //        ItemRemoval = new List<ItemRemoval>
            //        {
            //            new ItemRemoval { Code = "RemovalCode1", Description = "RemovalDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemRemoval { Code = "RemovalCode2", Description = "RemovalDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        },
            //        ItemAddition = new List<ItemAddition>
            //        {
            //            new ItemAddition { Code = "AdditionCode1", Description = "AdditionDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemAddition { Code = "AdditionCode2", Description = "AdditionDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        }
            //    }, new RulesValidationResult
            //    {
            //        id = 1,
            //        ruleName = "Rule1",
            //        ruleCode = "R001",
            //        errorMessage = "Error in Rule 1",
            //        longErrorMessage = "Detailed error message for Rule 1",
            //        priority = "High",
            //        solution = "Fix for Rule 1",
            //        action = "Take action for Rule 1",
            //        item = "Item1",
            //        itemDescription = "Description for Item1",
            //        ICDIsSigns = 1,
            //        HISCusId = "CusId1",
            //        Section = "SectionA",
            //        Qty = "1",
            //        ReplaceWith = "Replacement1",
            //        replaceCodes = new List<string> { "Code1", "Code2" },
            //        ReplaceWithHISCusId = "ReplaceCusId1",
            //        CustHISServiceType = "ServiceTypeA",
            //        ReplaceCusHISServiceType = "ReplaceServiceTypeA",
            //        CustHISDescription = "Description for ServiceTypeA",
            //        ReplaceCusHISDescription = "ReplaceDescriptionA",
            //        icdCode = new List<string> { "ICDCode1", "ICDCode2" },
            //        codes = new List<string> { "CodeA", "CodeB" },
            //        ruleRank = 1,
            //        ItemRemoval = new List<ItemRemoval>
            //        {
            //            new ItemRemoval { Code = "RemovalCode1", Description = "RemovalDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemRemoval { Code = "RemovalCode2", Description = "RemovalDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        },
            //        ItemAddition = new List<ItemAddition>
            //        {
            //            new ItemAddition { Code = "AdditionCode1", Description = "AdditionDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemAddition { Code = "AdditionCode2", Description = "AdditionDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        }
            //    }, new RulesValidationResult
            //    {
            //        id = 1,
            //        ruleName = "Rule1",
            //        ruleCode = "R001",
            //        errorMessage = "Error in Rule 1",
            //        longErrorMessage = "Detailed error message for Rule 1",
            //        priority = "High",
            //        solution = "Fix for Rule 1",
            //        action = "Take action for Rule 1",
            //        item = "Item1",
            //        itemDescription = "Description for Item1",
            //        ICDIsSigns = 1,
            //        HISCusId = "CusId1",
            //        Section = "SectionA",
            //        Qty = "1",
            //        ReplaceWith = "Replacement1",
            //        replaceCodes = new List<string> { "Code1", "Code2" },
            //        ReplaceWithHISCusId = "ReplaceCusId1",
            //        CustHISServiceType = "ServiceTypeA",
            //        ReplaceCusHISServiceType = "ReplaceServiceTypeA",
            //        CustHISDescription = "Description for ServiceTypeA",
            //        ReplaceCusHISDescription = "ReplaceDescriptionA",
            //        icdCode = new List<string> { "ICDCode1", "ICDCode2" },
            //        codes = new List<string> { "CodeA", "CodeB" },
            //        ruleRank = 1,
            //        ItemRemoval = new List<ItemRemoval>
            //        {
            //            new ItemRemoval { Code = "RemovalCode1", Description = "RemovalDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemRemoval { Code = "RemovalCode2", Description = "RemovalDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        },
            //        ItemAddition = new List<ItemAddition>
            //        {
            //            new ItemAddition { Code = "AdditionCode1", Description = "AdditionDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemAddition { Code = "AdditionCode2", Description = "AdditionDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        }
            //    }, new RulesValidationResult
            //    {
            //        id = 1,
            //        ruleName = "Rule1",
            //        ruleCode = "R001",
            //        errorMessage = "Error in Rule 1",
            //        longErrorMessage = "Detailed error message for Rule 1",
            //        priority = "High",
            //        solution = "Fix for Rule 1",
            //        action = "Take action for Rule 1",
            //        item = "Item1",
            //        itemDescription = "Description for Item1",
            //        ICDIsSigns = 1,
            //        HISCusId = "CusId1",
            //        Section = "SectionA",
            //        Qty = "1",
            //        ReplaceWith = "Replacement1",
            //        replaceCodes = new List<string> { "Code1", "Code2" },
            //        ReplaceWithHISCusId = "ReplaceCusId1",
            //        CustHISServiceType = "ServiceTypeA",
            //        ReplaceCusHISServiceType = "ReplaceServiceTypeA",
            //        CustHISDescription = "Description for ServiceTypeA",
            //        ReplaceCusHISDescription = "ReplaceDescriptionA",
            //        icdCode = new List<string> { "ICDCode1", "ICDCode2" },
            //        codes = new List<string> { "CodeA", "CodeB" },
            //        ruleRank = 1,
            //        ItemRemoval = new List<ItemRemoval>
            //        {
            //            new ItemRemoval { Code = "RemovalCode1", Description = "RemovalDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemRemoval { Code = "RemovalCode2", Description = "RemovalDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        },
            //        ItemAddition = new List<ItemAddition>
            //        {
            //            new ItemAddition { Code = "AdditionCode1", Description = "AdditionDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemAddition { Code = "AdditionCode2", Description = "AdditionDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        }
            //    }, new RulesValidationResult
            //    {
            //        id = 1,
            //        ruleName = "Rule1",
            //        ruleCode = "R001",
            //        errorMessage = "Error in Rule 1",
            //        longErrorMessage = "Detailed error message for Rule 1",
            //        priority = "High",
            //        solution = "Fix for Rule 1",
            //        action = "Take action for Rule 1",
            //        item = "Item1",
            //        itemDescription = "Description for Item1",
            //        ICDIsSigns = 1,
            //        HISCusId = "CusId1",
            //        Section = "SectionA",
            //        Qty = "1",
            //        ReplaceWith = "Replacement1",
            //        replaceCodes = new List<string> { "Code1", "Code2" },
            //        ReplaceWithHISCusId = "ReplaceCusId1",
            //        CustHISServiceType = "ServiceTypeA",
            //        ReplaceCusHISServiceType = "ReplaceServiceTypeA",
            //        CustHISDescription = "Description for ServiceTypeA",
            //        ReplaceCusHISDescription = "ReplaceDescriptionA",
            //        icdCode = new List<string> { "ICDCode1", "ICDCode2" },
            //        codes = new List<string> { "CodeA", "CodeB" },
            //        ruleRank = 1,
            //        ItemRemoval = new List<ItemRemoval>
            //        {
            //            new ItemRemoval { Code = "RemovalCode1", Description = "RemovalDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemRemoval { Code = "RemovalCode2", Description = "RemovalDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        },
            //        ItemAddition = new List<ItemAddition>
            //        {
            //            new ItemAddition { Code = "AdditionCode1", Description = "AdditionDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemAddition { Code = "AdditionCode2", Description = "AdditionDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        }
            //    }, new RulesValidationResult
            //    {
            //        id = 1,
            //        ruleName = "Rule1",
            //        ruleCode = "R001",
            //        errorMessage = "Error in Rule 1",
            //        longErrorMessage = "Detailed error message for Rule 1",
            //        priority = "High",
            //        solution = "Fix for Rule 1",
            //        action = "Take action for Rule 1",
            //        item = "Item1",
            //        itemDescription = "Description for Item1",
            //        ICDIsSigns = 1,
            //        HISCusId = "CusId1",
            //        Section = "SectionA",
            //        Qty = "1",
            //        ReplaceWith = "Replacement1",
            //        replaceCodes = new List<string> { "Code1", "Code2" },
            //        ReplaceWithHISCusId = "ReplaceCusId1",
            //        CustHISServiceType = "ServiceTypeA",
            //        ReplaceCusHISServiceType = "ReplaceServiceTypeA",
            //        CustHISDescription = "Description for ServiceTypeA",
            //        ReplaceCusHISDescription = "ReplaceDescriptionA",
            //        icdCode = new List<string> { "ICDCode1", "ICDCode2" },
            //        codes = new List<string> { "CodeA", "CodeB" },
            //        ruleRank = 1,
            //        ItemRemoval = new List<ItemRemoval>
            //        {
            //            new ItemRemoval { Code = "RemovalCode1", Description = "RemovalDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemRemoval { Code = "RemovalCode2", Description = "RemovalDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        },
            //        ItemAddition = new List<ItemAddition>
            //        {
            //            new ItemAddition { Code = "AdditionCode1", Description = "AdditionDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemAddition { Code = "AdditionCode2", Description = "AdditionDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        }
            //    }, new RulesValidationResult
            //    {
            //        id = 1,
            //        ruleName = "Rule1",
            //        ruleCode = "R001",
            //        errorMessage = "Error in Rule 1",
            //        longErrorMessage = "Detailed error message for Rule 1",
            //        priority = "High",
            //        solution = "Fix for Rule 1",
            //        action = "Take action for Rule 1",
            //        item = "Item1",
            //        itemDescription = "Description for Item1",
            //        ICDIsSigns = 1,
            //        HISCusId = "CusId1",
            //        Section = "SectionA",
            //        Qty = "1",
            //        ReplaceWith = "Replacement1",
            //        replaceCodes = new List<string> { "Code1", "Code2" },
            //        ReplaceWithHISCusId = "ReplaceCusId1",
            //        CustHISServiceType = "ServiceTypeA",
            //        ReplaceCusHISServiceType = "ReplaceServiceTypeA",
            //        CustHISDescription = "Description for ServiceTypeA",
            //        ReplaceCusHISDescription = "ReplaceDescriptionA",
            //        icdCode = new List<string> { "ICDCode1", "ICDCode2" },
            //        codes = new List<string> { "CodeA", "CodeB" },
            //        ruleRank = 1,
            //        ItemRemoval = new List<ItemRemoval>
            //        {
            //            new ItemRemoval { Code = "RemovalCode1", Description = "RemovalDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemRemoval { Code = "RemovalCode2", Description = "RemovalDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        },
            //        ItemAddition = new List<ItemAddition>
            //        {
            //            new ItemAddition { Code = "AdditionCode1", Description = "AdditionDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemAddition { Code = "AdditionCode2", Description = "AdditionDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        }
            //    }, new RulesValidationResult
            //    {
            //        id = 1,
            //        ruleName = "Rule1",
            //        ruleCode = "R001",
            //        errorMessage = "Error in Rule 1",
            //        longErrorMessage = "Detailed error message for Rule 1",
            //        priority = "High",
            //        solution = "Fix for Rule 1",
            //        action = "Take action for Rule 1",
            //        item = "Item1",
            //        itemDescription = "Description for Item1",
            //        ICDIsSigns = 1,
            //        HISCusId = "CusId1",
            //        Section = "SectionA",
            //        Qty = "1",
            //        ReplaceWith = "Replacement1",
            //        replaceCodes = new List<string> { "Code1", "Code2" },
            //        ReplaceWithHISCusId = "ReplaceCusId1",
            //        CustHISServiceType = "ServiceTypeA",
            //        ReplaceCusHISServiceType = "ReplaceServiceTypeA",
            //        CustHISDescription = "Description for ServiceTypeA",
            //        ReplaceCusHISDescription = "ReplaceDescriptionA",
            //        icdCode = new List<string> { "ICDCode1", "ICDCode2" },
            //        codes = new List<string> { "CodeA", "CodeB" },
            //        ruleRank = 1,
            //        ItemRemoval = new List<ItemRemoval>
            //        {
            //            new ItemRemoval { Code = "RemovalCode1", Description = "RemovalDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemRemoval { Code = "RemovalCode2", Description = "RemovalDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        },
            //        ItemAddition = new List<ItemAddition>
            //        {
            //            new ItemAddition { Code = "AdditionCode1", Description = "AdditionDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemAddition { Code = "AdditionCode2", Description = "AdditionDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        }
            //    }, new RulesValidationResult
            //    {
            //        id = 1,
            //        ruleName = "Rule1",
            //        ruleCode = "R001",
            //        errorMessage = "Error in Rule 1",
            //        longErrorMessage = "Detailed error message for Rule 1",
            //        priority = "High",
            //        solution = "Fix for Rule 1",
            //        action = "Take action for Rule 1",
            //        item = "Item1",
            //        itemDescription = "Description for Item1",
            //        ICDIsSigns = 1,
            //        HISCusId = "CusId1",
            //        Section = "SectionA",
            //        Qty = "1",
            //        ReplaceWith = "Replacement1",
            //        replaceCodes = new List<string> { "Code1", "Code2" },
            //        ReplaceWithHISCusId = "ReplaceCusId1",
            //        CustHISServiceType = "ServiceTypeA",
            //        ReplaceCusHISServiceType = "ReplaceServiceTypeA",
            //        CustHISDescription = "Description for ServiceTypeA",
            //        ReplaceCusHISDescription = "ReplaceDescriptionA",
            //        icdCode = new List<string> { "ICDCode1", "ICDCode2" },
            //        codes = new List<string> { "CodeA", "CodeB" },
            //        ruleRank = 1,
            //        ItemRemoval = new List<ItemRemoval>
            //        {
            //            new ItemRemoval { Code = "RemovalCode1", Description = "RemovalDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemRemoval { Code = "RemovalCode2", Description = "RemovalDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        },
            //        ItemAddition = new List<ItemAddition>
            //        {
            //            new ItemAddition { Code = "AdditionCode1", Description = "AdditionDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemAddition { Code = "AdditionCode2", Description = "AdditionDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        }
            //    }, new RulesValidationResult
            //    {
            //        id = 1,
            //        ruleName = "Rule1",
            //        ruleCode = "R001",
            //        errorMessage = "Error in Rule 1",
            //        longErrorMessage = "Detailed error message for Rule 1",
            //        priority = "High",
            //        solution = "Fix for Rule 1",
            //        action = "Take action for Rule 1",
            //        item = "Item1",
            //        itemDescription = "Description for Item1",
            //        ICDIsSigns = 1,
            //        HISCusId = "CusId1",
            //        Section = "SectionA",
            //        Qty = "1",
            //        ReplaceWith = "Replacement1",
            //        replaceCodes = new List<string> { "Code1", "Code2" },
            //        ReplaceWithHISCusId = "ReplaceCusId1",
            //        CustHISServiceType = "ServiceTypeA",
            //        ReplaceCusHISServiceType = "ReplaceServiceTypeA",
            //        CustHISDescription = "Description for ServiceTypeA",
            //        ReplaceCusHISDescription = "ReplaceDescriptionA",
            //        icdCode = new List<string> { "ICDCode1", "ICDCode2" },
            //        codes = new List<string> { "CodeA", "CodeB" },
            //        ruleRank = 1,
            //        ItemRemoval = new List<ItemRemoval>
            //        {
            //            new ItemRemoval { Code = "RemovalCode1", Description = "RemovalDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemRemoval { Code = "RemovalCode2", Description = "RemovalDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        },
            //        ItemAddition = new List<ItemAddition>
            //        {
            //            new ItemAddition { Code = "AdditionCode1", Description = "AdditionDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemAddition { Code = "AdditionCode2", Description = "AdditionDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        }
            //    }, new RulesValidationResult
            //    {
            //        id = 1,
            //        ruleName = "Rule1",
            //        ruleCode = "R001",
            //        errorMessage = "Error in Rule 1",
            //        longErrorMessage = "Detailed error message for Rule 1",
            //        priority = "High",
            //        solution = "Fix for Rule 1",
            //        action = "Take action for Rule 1",
            //        item = "Item1",
            //        itemDescription = "Description for Item1",
            //        ICDIsSigns = 1,
            //        HISCusId = "CusId1",
            //        Section = "SectionA",
            //        Qty = "1",
            //        ReplaceWith = "Replacement1",
            //        replaceCodes = new List<string> { "Code1", "Code2" },
            //        ReplaceWithHISCusId = "ReplaceCusId1",
            //        CustHISServiceType = "ServiceTypeA",
            //        ReplaceCusHISServiceType = "ReplaceServiceTypeA",
            //        CustHISDescription = "Description for ServiceTypeA",
            //        ReplaceCusHISDescription = "ReplaceDescriptionA",
            //        icdCode = new List<string> { "ICDCode1", "ICDCode2" },
            //        codes = new List<string> { "CodeA", "CodeB" },
            //        ruleRank = 1,
            //        ItemRemoval = new List<ItemRemoval>
            //        {
            //            new ItemRemoval { Code = "RemovalCode1", Description = "RemovalDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemRemoval { Code = "RemovalCode2", Description = "RemovalDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        },
            //        ItemAddition = new List<ItemAddition>
            //        {
            //            new ItemAddition { Code = "AdditionCode1", Description = "AdditionDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemAddition { Code = "AdditionCode2", Description = "AdditionDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        }
            //    }, new RulesValidationResult
            //    {
            //        id = 1,
            //        ruleName = "Rule1",
            //        ruleCode = "R001",
            //        errorMessage = "Error in Rule 1",
            //        longErrorMessage = "Detailed error message for Rule 1",
            //        priority = "High",
            //        solution = "Fix for Rule 1",
            //        action = "Take action for Rule 1",
            //        item = "Item1",
            //        itemDescription = "Description for Item1",
            //        ICDIsSigns = 1,
            //        HISCusId = "CusId1",
            //        Section = "SectionA",
            //        Qty = "1",
            //        ReplaceWith = "Replacement1",
            //        replaceCodes = new List<string> { "Code1", "Code2" },
            //        ReplaceWithHISCusId = "ReplaceCusId1",
            //        CustHISServiceType = "ServiceTypeA",
            //        ReplaceCusHISServiceType = "ReplaceServiceTypeA",
            //        CustHISDescription = "Description for ServiceTypeA",
            //        ReplaceCusHISDescription = "ReplaceDescriptionA",
            //        icdCode = new List<string> { "ICDCode1", "ICDCode2" },
            //        codes = new List<string> { "CodeA", "CodeB" },
            //        ruleRank = 1,
            //        ItemRemoval = new List<ItemRemoval>
            //        {
            //            new ItemRemoval { Code = "RemovalCode1", Description = "RemovalDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemRemoval { Code = "RemovalCode2", Description = "RemovalDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        },
            //        ItemAddition = new List<ItemAddition>
            //        {
            //            new ItemAddition { Code = "AdditionCode1", Description = "AdditionDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemAddition { Code = "AdditionCode2", Description = "AdditionDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        }
            //    }, new RulesValidationResult
            //    {
            //        id = 1,
            //        ruleName = "Rule1",
            //        ruleCode = "R001",
            //        errorMessage = "Error in Rule 1",
            //        longErrorMessage = "Detailed error message for Rule 1",
            //        priority = "High",
            //        solution = "Fix for Rule 1",
            //        action = "Take action for Rule 1",
            //        item = "Item1",
            //        itemDescription = "Description for Item1",
            //        ICDIsSigns = 1,
            //        HISCusId = "CusId1",
            //        Section = "SectionA",
            //        Qty = "1",
            //        ReplaceWith = "Replacement1",
            //        replaceCodes = new List<string> { "Code1", "Code2" },
            //        ReplaceWithHISCusId = "ReplaceCusId1",
            //        CustHISServiceType = "ServiceTypeA",
            //        ReplaceCusHISServiceType = "ReplaceServiceTypeA",
            //        CustHISDescription = "Description for ServiceTypeA",
            //        ReplaceCusHISDescription = "ReplaceDescriptionA",
            //        icdCode = new List<string> { "ICDCode1", "ICDCode2" },
            //        codes = new List<string> { "CodeA", "CodeB" },
            //        ruleRank = 1,
            //        ItemRemoval = new List<ItemRemoval>
            //        {
            //            new ItemRemoval { Code = "RemovalCode1", Description = "RemovalDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemRemoval { Code = "RemovalCode2", Description = "RemovalDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        },
            //        ItemAddition = new List<ItemAddition>
            //        {
            //            new ItemAddition { Code = "AdditionCode1", Description = "AdditionDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemAddition { Code = "AdditionCode2", Description = "AdditionDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        }
            //    }, new RulesValidationResult
            //    {
            //        id = 1,
            //        ruleName = "Rule1",
            //        ruleCode = "R001",
            //        errorMessage = "Error in Rule 1",
            //        longErrorMessage = "Detailed error message for Rule 1",
            //        priority = "High",
            //        solution = "Fix for Rule 1",
            //        action = "Take action for Rule 1",
            //        item = "Item1",
            //        itemDescription = "Description for Item1",
            //        ICDIsSigns = 1,
            //        HISCusId = "CusId1",
            //        Section = "SectionA",
            //        Qty = "1",
            //        ReplaceWith = "Replacement1",
            //        replaceCodes = new List<string> { "Code1", "Code2" },
            //        ReplaceWithHISCusId = "ReplaceCusId1",
            //        CustHISServiceType = "ServiceTypeA",
            //        ReplaceCusHISServiceType = "ReplaceServiceTypeA",
            //        CustHISDescription = "Description for ServiceTypeA",
            //        ReplaceCusHISDescription = "ReplaceDescriptionA",
            //        icdCode = new List<string> { "ICDCode1", "ICDCode2" },
            //        codes = new List<string> { "CodeA", "CodeB" },
            //        ruleRank = 1,
            //        ItemRemoval = new List<ItemRemoval>
            //        {
            //            new ItemRemoval { Code = "RemovalCode1", Description = "RemovalDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemRemoval { Code = "RemovalCode2", Description = "RemovalDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        },
            //        ItemAddition = new List<ItemAddition>
            //        {
            //            new ItemAddition { Code = "AdditionCode1", Description = "AdditionDescription1", ClientCode = "ClientCode1", ClientServiceType = "ClientServiceType1", ClientDescription = "ClientDescription1" },
            //            new ItemAddition { Code = "AdditionCode2", Description = "AdditionDescription2", ClientCode = "ClientCode2", ClientServiceType = "ClientServiceType2", ClientDescription = "ClientDescription2" }
            //        }
            //    },
            //    // Add more dummy data as needed
            //};

            //RuleValidations ruleValidations = new RuleValidations(dummyData);

            log4net.Config.BasicConfigurator.Configure();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new test());
        }
    }
}

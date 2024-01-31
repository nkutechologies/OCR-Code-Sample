using Newtonsoft.Json.Linq;
using System;

namespace Octopus.Common
{
    public static class JsonExtension
    {
        public static void ConvertCoordinatesToNewResolution(this JObject jsonObject, int newScreenWidth, int newScreenHeight, (int Width, int Height) currentDeviceResolution)
        {
            // Fetch original resolution from settings
            var originalResolution = currentDeviceResolution;
            int originalScreenWidth = originalResolution.Width;
            int originalScreenHeight = originalResolution.Height;

            JArray readingSections = (JArray)jsonObject["ReadingSection"];

            foreach (JToken section in readingSections)
            {
                string sectionName = section["Section"].ToString(); // Assuming "SectionName" is the key for the section name

                // Update Coordinates object (InitialX, InitialY)
                section.UpdateCoordinates(originalScreenWidth, originalScreenHeight, newScreenWidth, newScreenHeight, sectionName);

                // Check for ContextMenuActions and update
                section.UpdateContextMenuActions(originalScreenWidth, originalScreenHeight, newScreenWidth, newScreenHeight, sectionName);

                // Check for AdditionalActions and update
                section.UpdateAdditionalActions(originalScreenWidth, originalScreenHeight, newScreenWidth, newScreenHeight, sectionName);
            }
        }

        private static void UpdateCoordinates(this JToken section, int originalScreenWidth, int originalScreenHeight, int newScreenWidth, int newScreenHeight,string sectionName)
        {
            // Check if Coordinates object exists
            if (section["Coordinates"] != null)
            {
                JObject coordinates = (JObject)section["Coordinates"];
                //_log.Info("Section: "+section.ToString());
                UpdateXAndY(coordinates, originalScreenWidth, originalScreenHeight, newScreenWidth, newScreenHeight,sectionName);
            }
        }

        private static void UpdateContextMenuActions(this JToken section, int originalScreenWidth, int originalScreenHeight, int newScreenWidth, int newScreenHeight, string sectionName)
        {
            // Check if ContextMenuActions object exists
            if (section["ContextMenuActions"] != null)
            {
                JObject contextMenuActions = (JObject)section["ContextMenuActions"];
                //_log.Info("Section: " + section.ToString());
                UpdateXAndY(contextMenuActions, originalScreenWidth, originalScreenHeight, newScreenWidth, newScreenHeight,sectionName);
            }
        }

        private static void UpdateAdditionalActions(this JToken section, int originalScreenWidth, int originalScreenHeight, int newScreenWidth, int newScreenHeight, string sectionName)
        {
            // Check if AdditionalActions array exists
            if (section["AdditionalActions"] is JArray additionalActions)
            {
                foreach (JObject action in additionalActions)
                {
                    //_log.Info("Section: " + section.ToString());
                    UpdateXAndY(action, originalScreenWidth, originalScreenHeight, newScreenWidth, newScreenHeight,sectionName);
                }
            }
        }

        //private static void UpdateXAndY(JObject coordinates, int originalScreenWidth, int originalScreenHeight, int newScreenWidth, int newScreenHeight)
        //{
        //    double widthRatio = (double)newScreenWidth / originalScreenWidth;
        //    double heightRatio = (double)newScreenHeight / originalScreenHeight;

        //    Matrix transformationMatrix = new Matrix();
        //    transformationMatrix.Scale(widthRatio, heightRatio);

        //    Point originalPoint = new Point(
        //        double.Parse(coordinates["InitialX"].ToString()),
        //        double.Parse(coordinates["InitialY"].ToString())
        //    );

        //    Point transformedPoint = transformationMatrix.Transform(originalPoint);

        //    coordinates["InitialX"] = (int)Math.Round(transformedPoint.X);
        //    coordinates["InitialY"] = (int)Math.Round(transformedPoint.Y);
        //}
        //private static void UpdateXAndY(JObject coordinates, int originalScreenWidth, int originalScreenHeight, int newScreenWidth, int newScreenHeight)
        //{
        //    double widthRatio = (int)newScreenWidth / originalScreenWidth;
        //    double heightRatio = (int)newScreenHeight / originalScreenHeight;

        //    if (int.TryParse(coordinates["InitialX"].ToString(), out int initialX))
        //    {
        //        //_log.Info($"Original X: {initialX}");

        //        int newX = (int)Math.Round(initialX * widthRatio);
        //        //_log.Info($"Transformed X: {newX}");

        //        coordinates["InitialX"] = newX;
        //    }

        //    if (int.TryParse(coordinates["InitialY"].ToString(), out int initialY))
        //    {
        //        //_log.Info($"Original Y: {initialY}");

        //        int newY = (int)Math.Round(initialY * heightRatio);
        //        //_log.Info($"Transformed Y: {newY}");

        //        coordinates["InitialY"] = newY;
        //    }
        //}

        //private static void UpdateXAndY(JObject coordinates, int originalScreenWidth, int originalScreenHeight, int newScreenWidth, int newScreenHeight)
        //{
        //    if (int.TryParse(coordinates["InitialX"].ToString(), out int initialX))
        //    {
        //        double tX = (double)initialX / originalScreenWidth;
        //        coordinates["InitialX"] = (int)Math.Round(tX * newScreenWidth);
        //    }

        //    if (int.TryParse(coordinates["InitialY"].ToString(), out int initialY))
        //    {
        //        double tY = (double)initialY / originalScreenHeight;
        //        coordinates["InitialY"] = (int)Math.Round(tY * newScreenHeight);
        //    }
        //}

        private static void UpdateXAndY(JObject coordinates, int originalScreenWidth, int originalScreenHeight, int newScreenWidth, int newScreenHeight, string sectionName)
        {
            int fixedPointFactor = 10000; // You can adjust the precision based on your needs

            int AdjustCoordinate(int coordinate, int screenDimension, int newScreenDimension, int adjustment)
            {
                long t = (long)coordinate * fixedPointFactor / screenDimension;
                return (int)Math.Round((double)t * newScreenDimension / fixedPointFactor) + adjustment;
            }

            if (newScreenWidth == 1680 && newScreenHeight == 1050)
            {
                int xAdjustment = 0;
                int yAdjustment = 0;

                switch (sectionName)
                {
                    case "ClickOnSoapNotes":
                        xAdjustment = 90;
                        break;
                    case "CloseSoapNotes":
                        xAdjustment = -10;
                        break;
                    case "ClickOnComplain":
                        yAdjustment = 9;
                        break;
                    case "CloseObservation":
                        xAdjustment = -9;
                        break;
                    case "ClickOnCurrentDiagnosis":
                        yAdjustment = +10;
                        break;
                    case "MoveCursorToSearchBarOfDiagnosis":
                        yAdjustment =-10;
                        break;
                    case "CloseDiagnosisTab":
                        yAdjustment = -13;
                        xAdjustment =+ 35;
                        break;
                    case "ClickOnOtherActions":
                        xAdjustment = +100;
                        break;
                    case "ClickOnCeedValidation":
                        xAdjustment = +75;
                        break;
                    case "SaveXMLFile":
                        xAdjustment = +70;
                        yAdjustment = +10;
                        break;
                    case "CloseXMLViewer":
                        xAdjustment = -40;
                        break;
                    case "ClickOnRadiologyOrderSearchBar":
                        yAdjustment = +9;
                        break;
                    case "CloseRadiologyOrders":
                        xAdjustment = -8;
                        break;
                    case "ClickOnRadiologyOrderGridRow":
                        yAdjustment = +5;
                        break;
                    case "ClickOnLabOrders":
                        yAdjustment = +14;
                        break;
                    case "ClickOnLabOrderSearchBar":
                        yAdjustment = +9;
                        break;
                    case "ClickOnLabOrderGridRow":
                        yAdjustment = +5;
                        break;
                    case "CloseLabOrders":
                        xAdjustment = -8;
                        break;
                    case "MimicRemoveDiagnosisClick":
                        xAdjustment = +19;
                        yAdjustment = +5;
                        break;
                    case "ClickOnLabOrderShowMoreBtn":
                        yAdjustment = +15;
                        break;
                    case "ClickOnRadiologyShowMoreBtn":
                        yAdjustment = 20;
                        break;
                    case "ClickOnDeleteLabOrders":
                        xAdjustment = -5;
                        break;
                    case "ClickOnDeleteRadiology":
                        xAdjustment = -5;
                        break;
                    case "ClickOnDeleteLabOrdersWithoutShowMore":
                        xAdjustment = -14;
                        yAdjustment = 16;
                        break;
                    case "ClickOnDeleteRadiologyWithoutShowMore":
                        yAdjustment = 12;
                        xAdjustment = -9;
                        break;
                    case "MoveOctopusIconToDignosisPlusSign":
                        xAdjustment = 10;
                        yAdjustment = 7;
                        break;
                    case "MoveOctopusIconToMarkToBillBtn":
                        xAdjustment = 50;
                        break;
                    case "ClickOnFollowUp":
                        yAdjustment = 10;
                        xAdjustment = 70;
                        break;
                    case "CloseFollowUpVisitForInsurance":
                        yAdjustment = 10;
                        xAdjustment = -5;
                        break;
                    // Add more cases as needed

                    default:
                        break;
                }

                if (int.TryParse(coordinates["InitialX"].ToString(), out int initialX))
                {
                    coordinates["InitialX"] = AdjustCoordinate(initialX, originalScreenWidth, newScreenWidth, xAdjustment);
                }

                if (int.TryParse(coordinates["InitialY"].ToString(), out int initialY))
                {
                    coordinates["InitialY"] = AdjustCoordinate(initialY, originalScreenHeight, newScreenHeight, yAdjustment);
                }
            }
            else if (newScreenWidth == 1600 && newScreenHeight == 900)
            {
                int xAdjustment = 0;
                int yAdjustment = 0;

                switch (sectionName)
                {
                    case "FetchPatientMPI":
                        yAdjustment = 30;
                        break;
                    case "ClickOnSoapNotes":
                        xAdjustment = 123;
                        yAdjustment = 27;
                        break;
                    case "FetchGender":
                        yAdjustment = +22;
                        break;
                    case "CloseSoapNotes":
                        xAdjustment = 305;
                        break;
                    case "ClickOnObservation":
                        xAdjustment = 157;
                        yAdjustment = 57;
                        break;
                    case "ClickOnComplain":
                        yAdjustment = 50;
                        break;
                    case "CloseObservation":
                        xAdjustment = 295;
                        yAdjustment = 9;
                        break;
                    case "ClickOnCurrentDiagnosis":
                        yAdjustment = 55;
                        xAdjustment = 40;
                        break;
                    case "MoveCursorToSearchBarOfDiagnosis":
                        yAdjustment = 55;
                        break;
                    case "ClickOnRowItemFromDignosisGrid":
                        yAdjustment = 65;
                        break;
                    case "CloseDiagnosisTab":
                        xAdjustment = 210;
                        yAdjustment = 50;
                        break;
                    case "ClickOnOtherActions":
                        xAdjustment = +145;
                        yAdjustment = 29;
                        break;
                    case "ClickOnCeedValidation":
                        xAdjustment = 150;
                        yAdjustment = +29;
                        break;
                    case "ClickOnViewXML":
                        yAdjustment = 15;
                        xAdjustment = 219;
                        break;
                    case "ClickOnDownloadXML":
                        yAdjustment = 19;
                        xAdjustment = 259;
                        break;
                    case "CopyFolderPath":
                        xAdjustment = 50;
                        break;
                    case "SaveXMLFile":
                        xAdjustment = +123;
                        yAdjustment = 65;
                        break;
                    case "CloseXMLViewer":
                        xAdjustment = 267;
                        yAdjustment = 21;
                        break;
                    case "CloseCEEDValidationWindow":
                        xAdjustment = 308;
                        yAdjustment = 5;
                        break;
                    case "ClickOnRadiologyOrderSearchBar":
                        yAdjustment = 30;
                        break;
                    case "CloseRadiologyOrders":
                        xAdjustment = 296;
                        yAdjustment = 9;
                        break;
                    case "ClickOnRadiologyOrderGridRow":
                        yAdjustment = 46;
                        break;
                    case "ClickOnLabOrders":
                        yAdjustment = 79;
                        xAdjustment = 250;
                        break;
                    case "ClickOnLabOrderSearchBar":
                        yAdjustment = +28;
                        break;
                    case "ClickOnLabOrderGridRow":
                        yAdjustment = +30;
                        break;
                    case "CloseLabOrders":
                        xAdjustment = 300;
                        yAdjustment = 10;
                        break;
                    case "ClickOnRadiologyOrders":
                        yAdjustment = 90;
                        xAdjustment = 250;
                        break;
                    case "MimicRemoveDiagnosisClick":
                        xAdjustment = 190;
                        yAdjustment = 129;
                        break;
                    case "ClickOnLabOrderShowMoreBtn":
                        yAdjustment = 76;
                        xAdjustment = 295;
                        break;
                    case "ClickOnRadiologyShowMoreBtn":
                        yAdjustment = 81;
                        xAdjustment =312;
                        break;
                    case "ClickOnDeleteLabOrders":
                        xAdjustment = 299;
                        yAdjustment = 29;
                        break;
                    case "ClickOnDeleteRadiology":
                        xAdjustment = 299;
                        yAdjustment = 38;
                        break;
                    case "ClickOnDeleteLabOrdersWithoutShowMore":
                        xAdjustment = 293;
                        yAdjustment = 92;
                        break;
                    case "ClickOnDeleteRadiologyWithoutShowMore":
                        xAdjustment = 300;
                        yAdjustment = 87;
                        break;
                    case "MoveOctopusIconToDignosisPlusSign":
                        xAdjustment = 210;
                        yAdjustment = 46;
                        break;
                    case "MoveOctopusIconToMarkToBillBtn":
                        xAdjustment = 77;
                        yAdjustment = 28;
                        break;
                    case "ClickOnFollowUp":
                        xAdjustment = 150;
                        yAdjustment = 70;
                        break;
                    case "CopyPayerTypeForSelfPayer":
                        xAdjustment = 150;
                        yAdjustment = 39;
                        break;
                    case "CloseFollowUpVisitForInsurance":
                        xAdjustment = 268;
                        yAdjustment = 45;
                        break;
                    // Add more cases as needed

                    default:
                        break;
                }

                if (int.TryParse(coordinates["InitialX"].ToString(), out int initialX))
                {
                    coordinates["InitialX"] = AdjustCoordinate(initialX, originalScreenWidth, newScreenWidth, xAdjustment);
                }

                if (int.TryParse(coordinates["InitialY"].ToString(), out int initialY))
                {
                    coordinates["InitialY"] = AdjustCoordinate(initialY, originalScreenHeight, newScreenHeight, yAdjustment);
                }
            }
            else if (newScreenWidth == 1440 && newScreenHeight == 900)
            {
                int xAdjustment = 0;
                int yAdjustment = 0;

                switch (sectionName)
                {
                    case "FetchPatientMPI":
                        yAdjustment = 30;
                        xAdjustment = 29;
                        break;
                    case "ClickOnSoapNotes":
                        xAdjustment = 172;
                        yAdjustment = 27;
                        break;
                    case "FetchGender":
                        yAdjustment = +22;
                        break;
                    case "CloseSoapNotes":
                        xAdjustment = 271;
                        break;
                    case "ClickOnObservation":
                        xAdjustment = 157;
                        yAdjustment = 57;
                        break;
                    case "ClickOnComplain":
                        yAdjustment = 50;
                        break;
                    case "CloseObservation":
                        xAdjustment = 252;
                        yAdjustment = 9;
                        break;
                    case "ClickOnCurrentDiagnosis":
                        yAdjustment = 55;
                        xAdjustment = 40;
                        break;
                    case "MoveCursorToSearchBarOfDiagnosis":
                        yAdjustment = 55;
                        break;
                    case "ClickOnRowItemFromDignosisGrid":
                        yAdjustment = 65;
                        break;
                    case "CloseDiagnosisTab":
                        xAdjustment = 220;
                        yAdjustment = 47;
                        break;
                    case "ClickOnOtherActions":
                        xAdjustment = +209;
                        yAdjustment = 29;
                        break;
                    case "ClickOnCeedValidation":
                        xAdjustment = 213;
                        yAdjustment = +29;
                        break;
                    case "ClickOnViewXML":
                        yAdjustment = 15;
                        xAdjustment = 219;
                        break;
                    case "ClickOnDownloadXML":
                        yAdjustment = 19;
                        xAdjustment = 259;
                        break;
                    case "CopyFolderPath":
                        xAdjustment = 67;
                        break;
                    case "SaveXMLFile":
                        xAdjustment = +141;
                        yAdjustment = 65;
                        break;
                    case "CloseXMLViewer":
                        xAdjustment = 224;
                        yAdjustment = 21;
                        break;
                    case "CloseCEEDValidationWindow":
                        xAdjustment = 275;
                        yAdjustment = 5;
                        break;
                    case "ClickOnRadiologyOrderSearchBar":
                        yAdjustment = 30;
                        break;
                    case "CloseRadiologyOrders":
                        xAdjustment = 260;
                        yAdjustment = 9;
                        break;
                    case "ClickOnRadiologyOrderGridRow":
                        yAdjustment = 46;
                        break;
                    case "ClickOnLabOrders":
                        yAdjustment = 79;
                        xAdjustment = 250;
                        break;
                    case "ClickOnLabOrderSearchBar":
                        yAdjustment = +28;
                        break;
                    case "ClickOnLabOrderGridRow":
                        yAdjustment = +30;
                        break;
                    case "CloseLabOrders":
                        xAdjustment = 263;
                        yAdjustment = 10;
                        break;
                    case "ClickOnRadiologyOrders":
                        yAdjustment = 90;
                        xAdjustment = 250;
                        break;
                    case "MimicRemoveDiagnosisClick":
                        xAdjustment = 210;
                        yAdjustment = 129;
                        break;
                    case "ClickOnLabOrderShowMoreBtn":
                        yAdjustment = 76;
                        xAdjustment = 275;
                        break;
                    case "ClickOnRadiologyShowMoreBtn":
                        yAdjustment = 78;
                        xAdjustment = 269;
                        break;
                    case "ClickOnDeleteLabOrders":
                        xAdjustment = 263;
                        yAdjustment = 29;
                        break;
                    case "ClickOnDeleteRadiology":
                        xAdjustment = 267;
                        yAdjustment = 38;
                        break;
                    case "ClickOnDeleteLabOrdersWithoutShowMore":
                        xAdjustment = 258;
                        yAdjustment= 96;
                        break;
                    case "ClickOnDeleteRadiologyWithoutShowMore":
                        xAdjustment = 258;
                        yAdjustment = 90;
                        break;
                    case "MoveOctopusIconToDignosisPlusSign":
                        xAdjustment = 189;
                        yAdjustment = 46;
                        break;
                    case "MoveOctopusIconToMarkToBillBtn":
                        xAdjustment = 104;
                        yAdjustment = 28;
                        break;
                    case "ClickOnFollowUp":
                        xAdjustment = 220;
                        yAdjustment = 70;
                        break;
                    case "CopyPayerTypeForSelfPayer":
                        xAdjustment = 150;
                        yAdjustment = 39;
                        break;
                    case "CloseFollowUpVisitForInsurance":
                        xAdjustment = 245;
                        yAdjustment = 45;
                        break;
                    // Add more cases as needed

                    default:
                        break;
                }

                if (int.TryParse(coordinates["InitialX"].ToString(), out int initialX))
                {
                    coordinates["InitialX"] = AdjustCoordinate(initialX, originalScreenWidth, newScreenWidth, xAdjustment);
                }

                if (int.TryParse(coordinates["InitialY"].ToString(), out int initialY))
                {
                    coordinates["InitialY"] = AdjustCoordinate(initialY, originalScreenHeight, newScreenHeight, yAdjustment);
                }
            }
            else if (newScreenWidth == 1400 && newScreenHeight == 1050)
            {
                int xAdjustment = 0;
                int yAdjustment = 0;

                switch (sectionName)
                {
                    case "FetchPatientMPI":
                        yAdjustment = 7;
                        xAdjustment = 29;
                        break;
                    case "ClickOnSoapNotes":
                        xAdjustment = 189;
                        yAdjustment = 10;
                        break;
                    case "FetchGender":
                        yAdjustment = +10;
                        break;
                    case "CloseSoapNotes":
                        xAdjustment = -14;
                        yAdjustment = -5;
                        break;
                    case "ClickOnObservation":
                        xAdjustment = 10;
                        yAdjustment = 5;
                        break;
                    case "ClickOnComplain":
                        yAdjustment = 10;
                        break;
                    case "CloseObservation":
                        xAdjustment = -39;
                        yAdjustment = 0;
                        break;
                    case "ClickOnCurrentDiagnosis":
                        yAdjustment = 25;
                        xAdjustment = 30;
                        break;
                    case "MoveCursorToSearchBarOfDiagnosis":
                        yAdjustment = -10;
                        break;
                    case "ClickOnRowItemFromDignosisGrid":
                        yAdjustment = -5;
                        break;
                    case "CloseDiagnosisTab":
                        xAdjustment = 88;
                        yAdjustment = -5;
                        break;
                    case "ClickOnOtherActions":
                        xAdjustment = +235;
                        yAdjustment = 10;
                        break;
                    case "ClickOnCeedValidation":
                        xAdjustment = 213;
                        yAdjustment = 4;
                        break;
                    case "ClickOnViewXML":
                        yAdjustment = 11;
                        xAdjustment = 57;
                        break;
                    case "ClickOnDownloadXML":
                        yAdjustment = 8;
                        xAdjustment = -35;
                        break;
                    case "CopyFolderPath":
                        xAdjustment = 67;
                        break;
                    case "SaveXMLFile":
                        xAdjustment = +155;
                        yAdjustment = 20;
                        break;
                    case "CloseXMLViewer":
                        xAdjustment = -72;
                        yAdjustment = -5;
                        break;
                    case "CloseCEEDValidationWindow":
                        xAdjustment = -16;
                        yAdjustment = 5;
                        break;
                    case "ClickOnRadiologyOrderSearchBar":
                        yAdjustment = 13;
                        break;
                    case "CloseRadiologyOrders":
                        xAdjustment = -30;
                        yAdjustment = 0;
                        break;
                    case "ClickOnRadiologyOrderGridRow":
                        yAdjustment = 8;
                        break;
                    case "ClickOnLabOrders":
                        yAdjustment = 25;
                        xAdjustment = 100;
                        break;
                    case "ClickOnLabOrderSearchBar":
                        yAdjustment = 11;
                        break;
                    case "ClickOnLabOrderGridRow":
                        yAdjustment = 8;
                        break;
                    case "CloseLabOrders":
                        xAdjustment = -31;
                        yAdjustment = 0;
                        break;
                    case "ClickOnRadiologyOrders":
                        yAdjustment = 10;
                        xAdjustment = 10;
                        break;
                    case "MimicRemoveDiagnosisClick":
                        xAdjustment = 73;
                        yAdjustment = 5;
                        break;
                    case "ClickOnLabOrderShowMoreBtn":
                        xAdjustment = -25;
                        yAdjustment = 8;
                        break;
                    case "ClickOnRadiologyShowMoreBtn":
                        yAdjustment = 17;
                        xAdjustment = -22;
                        break;
                    case "ClickOnDeleteLabOrders":
                        xAdjustment = -30;
                        yAdjustment = 0;
                        break;
                    case "ClickOnDeleteRadiology":
                        xAdjustment = -25;
                        yAdjustment = 11;
                        break;
                    case "ClickOnDeleteLabOrdersWithoutShowMore":
                        xAdjustment = -32;
                        yAdjustment = 19;
                        break;
                    case "ClickOnDeleteRadiologyWithoutShowMore":
                        xAdjustment = -28;
                        yAdjustment = 20;
                        break;
                    case "MoveOctopusIconToDignosisPlusSign":
                        xAdjustment = 45;
                        yAdjustment = 9;
                        break;
                    case "MoveOctopusIconToMarkToBillBtn":
                        xAdjustment = 113;
                        yAdjustment = 6;
                        break;
                    case "ClickOnFollowUp":
                        xAdjustment = 190;
                        yAdjustment = 25;
                        break;
                    case "CloseFollowUpVisitForInsurance":
                        xAdjustment = -7;
                        yAdjustment = 9;
                        break;
                    // Add more cases as needed

                    default:
                        break;
                }

                if (int.TryParse(coordinates["InitialX"].ToString(), out int initialX))
                {
                    coordinates["InitialX"] = AdjustCoordinate(initialX, originalScreenWidth, newScreenWidth, xAdjustment);
                }

                if (int.TryParse(coordinates["InitialY"].ToString(), out int initialY))
                {
                    coordinates["InitialY"] = AdjustCoordinate(initialY, originalScreenHeight, newScreenHeight, yAdjustment);
                }
            }
            else
            {
                if (int.TryParse(coordinates["InitialX"].ToString(), out int initialX))
                {
                    coordinates["InitialX"] = AdjustCoordinate(initialX, originalScreenWidth, newScreenWidth, 0);
                }

                if (int.TryParse(coordinates["InitialY"].ToString(), out int initialY))
                {
                    coordinates["InitialY"] = AdjustCoordinate(initialY, originalScreenHeight, newScreenHeight, 0);
                }
            }
        }

        //private static void UpdateXAndY(JObject coordinates, int originalScreenWidth, int originalScreenHeight, int newScreenWidth, int newScreenHeight)
        //{
        //    if (int.TryParse(coordinates["InitialX"].ToString(), out int initialX) && int.TryParse(coordinates["InitialY"].ToString(), out int initialY))
        //    {
        //        // Define the system of linear equations
        //        double[,] coefficients = {
        //    { originalScreenWidth, 0 },
        //    { 0, originalScreenHeight }
        //};

        //        double[] constants = { initialX, initialY };

        //        // Solve the system of linear equations
        //        double[] solutions = SolveLinearEquations(coefficients, constants);

        //        // Update the coordinates
        //        coordinates["InitialX"] = (int)Math.Round(solutions[0] * newScreenWidth);
        //        coordinates["InitialY"] = (int)Math.Round(solutions[1] * newScreenHeight);
        //    }
        //}

        //private static double[] SolveLinearEquations(double[,] coefficients, double[] constants)
        //{
        //    int rows = coefficients.GetLength(0);
        //    int cols = coefficients.GetLength(1);

        //    if (rows != cols || rows != constants.Length)
        //    {
        //        throw new ArgumentException("Invalid system of linear equations");
        //    }

        //    var matrix = Matrix<double>.Build.DenseOfArray(coefficients);
        //    var vector = Vector<double>.Build.Dense(constants);

        //    var solutions = matrix.Solve(vector);

        //    return solutions.ToArray();
        //}
    }
}

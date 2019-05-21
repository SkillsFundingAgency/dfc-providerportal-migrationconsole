using Dfc.CourseDirectory.ApprenticeshipMigrationTool.Helpers;
using Dfc.CourseDirectory.ApprenticeshipMigrationTool.Models;
using Dfc.CourseDirectory.Models.Enums;
using Dfc.CourseDirectory.Models.Interfaces.Providers;
using Dfc.CourseDirectory.Models.Models.Apprenticeships;
using Dfc.CourseDirectory.Models.Models.Regions;
using Dfc.CourseDirectory.Models.Models.Venues;
using Dfc.CourseDirectory.Services.Interfaces.ProviderService;
using Dfc.CourseDirectory.Services.Interfaces.VenueService;
using Dfc.CourseDirectory.Services.ProviderService;
using Dfc.CourseDirectory.Services.VenueService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dfc.CourseDirectory.ApprenticeshipMigrationTool
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Configuration 

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddTransient((provider) => new HttpClient())
                .Configure<VenueServiceSettings>(venueServiceSettingsOptions =>
                {
                    venueServiceSettingsOptions.ApiUrl = configuration.GetValue<string>("VenueServiceSettings:ApiUrl");
                    venueServiceSettingsOptions.ApiKey = configuration.GetValue<string>("VenueServiceSettings:ApiKey");
                }
                )
                .AddScoped<IVenueService, VenueService>()
                .Configure<ProviderServiceSettings>(providerServiceSettingsOptions =>
                {
                    providerServiceSettingsOptions.ApiUrl = configuration.GetValue<string>("ProviderServiceSettings:ApiUrl");
                    providerServiceSettingsOptions.ApiKey = configuration.GetValue<string>("ProviderServiceSettings:ApiKey");
                }
                )
                .AddScoped<IProviderService, ProviderService>()
                .BuildServiceProvider();

            // Configure console logging
            serviceProvider
                .GetService<ILoggerFactory>();
            //.AddConsole(LogLevel.Debug); // Not Working and not needed

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogDebug("Starting Dfc.CourseDirectory.ApprenticeshipMigrationTool application");

            // Initialise the services
            var venueService = serviceProvider.GetService<IVenueService>();
            var providerService = serviceProvider.GetService<IProviderService>();

            logger.LogDebug("Log test.");


            string connectionString = configuration.GetConnectionString("DefaultConnection");
            bool AutomatedMode = configuration.GetValue<bool>("AutomatedMode");
            bool GenerateJsonFilesLocally = configuration.GetValue<bool>("GenerateJsonFilesLocally");
            bool GenerateReportFilesLocally = configuration.GetValue<bool>("GenerateReportFilesLocally");
            string JsonApprenticeshipFilesPath = configuration.GetValue<string>("JsonApprenticeshipFilesPath");
            string selectionOfProvidersFileName = configuration.GetValue<string>("SelectionOfProvidersFileName");
            DeploymentEnvironment deploymentEnvironment = configuration.GetValue<DeploymentEnvironment>("DeploymentEnvironment");
            //TransferMethod transferMethod = configuration.GetValue<TransferMethod>("TransferMethod");

            bool UpdateProvider = configuration.GetValue<bool>("UpdateProvider");
            int VenueBasedRadius = configuration.GetValue<int>("VenueBasedRadius");
            int RegionBasedRadius = configuration.GetValue<int>("RegionBasedRadius");
            int SubRegionBasedRadius = configuration.GetValue<int>("SubRegionBasedRadius");
            int RegionSubRegionRangeRadius = configuration.GetValue<int>("RegionSubRegionRangeRadius");

            #endregion

            #region Get User Input and Set Variables

            string adminReport = "                         Admin Report " + Environment.NewLine;
            adminReport += "________________________________________________________________________________" + Environment.NewLine + Environment.NewLine;

            var providerUKPRNList = new List<int>();
            int courseTransferId = 0;
            bool goodToTransfer = false;
            TransferMethod transferMethod = TransferMethod.Undefined;
            int? singleProviderUKPRN = null;
            string bulkUploadFileName = string.Empty;

            if (AutomatedMode)
            {
                Console.WriteLine("The Migration Apprenticeship Tool is running in Automated Mode." + Environment.NewLine + "Please, do not close this window until \"Migration completed\" message is displayed." + Environment.NewLine);

                string errorMessageGetCourses = string.Empty;
                providerUKPRNList = DataHelper.GetProviderUKPRNs(connectionString, out errorMessageGetCourses);
                if (!string.IsNullOrEmpty(errorMessageGetCourses))
                {
                    adminReport += $"* ATTENTION * { errorMessageGetCourses }" + Environment.NewLine;
                }
                else
                {
                    goodToTransfer = true;
                    transferMethod = TransferMethod.CourseMigrationTool;
                }
            }
            else
            {
                Console.WriteLine("---------------------------");
                Console.WriteLine(" APPRENTICESHIPS MIGRATION");
                Console.WriteLine("---------------------------");
                Console.WriteLine("Please enter valid UKPRN to migrate apprenticeships for a single Provider" + Environment.NewLine + "or \"s\" to migrate apprenticeships for a selection of Providers:" +
                                   Environment.NewLine + "(Ensure that you have created a folder named 'ProviderSelections' and placed your .csv file in it.)");
                string providerInput = Console.ReadLine();

                if (string.IsNullOrEmpty(providerInput))
                {
                    Console.WriteLine("Please next time enter a value.");
                }
                else if (providerInput.Equals("s", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Migrate selection of Providers from .CSV file
                    string ProviderSelectionsPath = string.Format(@"{0}\ProviderSelections", JsonApprenticeshipFilesPath);
                    if (!Directory.Exists(ProviderSelectionsPath))
                        Directory.CreateDirectory(ProviderSelectionsPath);
                    string selectionOfProviderFile = string.Format(@"{0}\{1}", ProviderSelectionsPath, selectionOfProvidersFileName);
                    using (StreamReader reader = new StreamReader(selectionOfProviderFile))
                    {
                        string line = null;
                        while (null != (line = reader.ReadLine()))
                        {
                            string[] providers = line.Split(',');
                            foreach (var provider in providers)
                            {
                                if (CheckForValidUKPRN(provider))
                                {
                                    providerUKPRNList.Add(Convert.ToInt32(provider));

                                    goodToTransfer = true;
                                    bulkUploadFileName = selectionOfProvidersFileName;
                                    transferMethod = TransferMethod.CourseMigrationToolCsvFile;
                                }
                                else if (string.IsNullOrEmpty(provider))
                                {
                                    // We don't want to know about the empty spaces on splitting
                                }
                                else
                                {
                                    // Log invalid providers
                                    adminReport += $">>> The following ( { provider } ) is not valid UKPRN." + Environment.NewLine + Environment.NewLine;
                                }
                            }
                        }
                    }


                }
                else if (CheckForValidUKPRN(providerInput))
                {
                    // Migrate single Provider
                    providerUKPRNList.Add(Convert.ToInt32(providerInput));
                    goodToTransfer = true;
                    transferMethod = TransferMethod.CourseMigrationToolSingleUkprn;
                    singleProviderUKPRN = Convert.ToInt32(providerInput);
                }
                else
                {
                    Console.WriteLine("You have to enter either valid UKPRN (which must be 8 digit number starting with a 1 e.g. 10000364) or \"s\" for selection of Providers");
                }
            }

            // Auditing to be decided
            //if (goodToTransfer)
            //{
            //    string errorMessageCourseTransferAdd = string.Empty;
            //    if (providerUKPRNList != null && providerUKPRNList.Count > 0)
            //    {
            //        DataHelper.CourseTransferAdd(connectionString,
            //                                        DateTime.Now,
            //                                        (int)transferMethod,
            //                                        (int)deploymentEnvironment,
            //                                        string.Empty,
            //                                        "DFC – Course Migration Tool",
            //                                        singleProviderUKPRN,
            //                                        out errorMessageCourseTransferAdd,
            //                                        out courseTransferId);
            //    }
            //    if (!string.IsNullOrEmpty(errorMessageCourseTransferAdd)) adminReport += errorMessageCourseTransferAdd + Environment.NewLine;

            //    if (courseTransferId.Equals(-1))
            //    {
            //        adminReport += $"We cannot get the BatchNumber (CourseTransferId), so migration will be terminated. Number of UKPRNs ( { providerUKPRNList?.Count } )" + Environment.NewLine;
            //        providerUKPRNList = null;
            //    }
            //}


            Stopwatch adminStopWatch = new Stopwatch();
            adminStopWatch.Start();
            Stopwatch provStopWatch = new Stopwatch();
            provStopWatch.Start();

            int CountProviders = 0;


            #endregion

            foreach (var providerUKPRN in providerUKPRNList)
            {
                CountProviders++;
                string providerReport = "                         Migration Report " + Environment.NewLine;

                // GetProviderDetailsByUKPRN
                string errorMessageGetProviderDetailsByUKPRN = string.Empty;
                var provider = DataHelper.GetProviderDetailsByUKPRN(providerUKPRN, connectionString, out errorMessageGetProviderDetailsByUKPRN);
                var ProviderGuidId = new Guid();
                string providerUkprnLine = "Provider - " + providerUKPRN + " - " + provider.ProviderName;
                Console.WriteLine(providerUkprnLine);
                adminReport += "_________________________________________________________________________________________________________" + Environment.NewLine;
                adminReport += Environment.NewLine + providerUkprnLine + Environment.NewLine;

                if (!string.IsNullOrEmpty(errorMessageGetProviderDetailsByUKPRN))
                {
                    adminReport += $"* ATTENTION * { errorMessageGetProviderDetailsByUKPRN }" + Environment.NewLine;
                }
                else
                {
                    var providerCriteria = new ProviderSearchCriteria(providerUKPRN.ToString());
                    var providerResult = Task.Run(async () => await providerService.GetProviderByPRNAsync(providerCriteria)).Result;

                    if (providerResult.IsSuccess && providerResult.HasValue)
                    {
                        var providers = providerResult.Value.Value;
                        if (providers.Count().Equals(1))
                        {
                            var providerToUpdate = providers.FirstOrDefault();
                            ProviderGuidId = providerToUpdate.id; // We need our Provider GUID id

                            #region  Update Provider

                            if (UpdateProvider)
                            {
                                // Commented out fields are not updated
                                //providerToUpdate.ProviderName = provider.ProviderName;
                                //providerToUpdate.TradingName = provider.TradingName;
                                providerToUpdate.ProviderId = provider.ProviderId;
                                providerToUpdate.UPIN = provider.UPIN;
                                providerToUpdate.MarketingInformation = provider.MarketingInformation;

                                //if (!string.IsNullOrEmpty(provider.ProviderNameAlias))
                                //{
                                //    if (providerToUpdate.ProviderAliases != null && providerToUpdate.ProviderAliases[0].ProviderAlias == null)
                                //    {
                                //        var providerAlias = new Provideralias();
                                //        providerAlias.ProviderAlias = provider.ProviderNameAlias;
                                //        providerAlias.LastUpdated = DateTime.Now;
                                //        providerToUpdate.ProviderAliases = new IProvideralias[] { providerAlias };
                                //    }
                                //}

                                var ApprenticeshipProviderContact = new Providercontact();
                                ApprenticeshipProviderContact.ContactType = "A";
                                ApprenticeshipProviderContact.ContactTelephone1 = provider.Telephone;
                                ApprenticeshipProviderContact.ContactEmail = provider.Email;
                                ApprenticeshipProviderContact.ContactWebsiteAddress = provider.Website;
                                ApprenticeshipProviderContact.LastUpdated = DateTime.Now;
                                if (providerToUpdate.ProviderContact == null)
                                {
                                    providerToUpdate.ProviderContact = new IProvidercontact[] { ApprenticeshipProviderContact };
                                }
                                else
                                {
                                    providerToUpdate.ProviderContact = providerToUpdate.ProviderContact.Append(ApprenticeshipProviderContact).ToArray();
                                }


                                if (GenerateJsonFilesLocally)
                                {
                                    var providerToUpdateJson = JsonConvert.SerializeObject(providerToUpdate);
                                    string jsonUpdateProviderPathFileName = string.Format("{0}-UpdateProvider-{1}.json", DateTime.Now.ToString("yyMMdd-HHmmss"), providerToUpdate.UnitedKingdomProviderReferenceNumber);
                                    string UpdateProviderPath = string.Format(@"{0}\UpdateProvider", JsonApprenticeshipFilesPath);
                                    if (!Directory.Exists(UpdateProviderPath))
                                        Directory.CreateDirectory(UpdateProviderPath);
                                    File.WriteAllText(string.Format(@"{0}\{1}", UpdateProviderPath, jsonUpdateProviderPathFileName), providerToUpdateJson);
                                }
                                else
                                {
                                    // Call ProviderService API to update provider
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            providerReport += $"We CANNOT IDENTIFY the Provider - " + Environment.NewLine + Environment.NewLine;
                        }
                    }
                    else
                    {
                        providerReport += $"ERROR on GETTING the Provider - { providerResult.Error }" + Environment.NewLine + Environment.NewLine;
                    }

                    // Get Apprenticeships by ProviderId                    
                    string errorMessageGetApprenticeshipsByProviderId = string.Empty;
                    var apprenticeships = DataHelper.GetApprenticeshipsByProviderId(provider.ProviderId ?? 0, connectionString, out errorMessageGetApprenticeshipsByProviderId);
                    if (!string.IsNullOrEmpty(errorMessageGetApprenticeshipsByProviderId))
                    {
                        adminReport += $"* ATTENTION * { errorMessageGetApprenticeshipsByProviderId }" + Environment.NewLine;
                    }
                    else
                    {
                        foreach (var apprenticeship in apprenticeships)
                        {
                            // // Mapp Apprenticeships
                            apprenticeship.id = Guid.NewGuid();
                            apprenticeship.ProviderId = ProviderGuidId;
                            apprenticeship.ProviderUKPRN = providerUKPRN;

                            apprenticeship.RecordStatus = RecordStatus.Live;

                            apprenticeship.CreatedDate = DateTime.Now;
                            apprenticeship.CreatedBy = "DFC – Apprenticeship Migration Tool";
                            adminReport += "____________________________________________________________________" + Environment.NewLine;
                            // Get Framework/Standard GUID id => ???? Call ReferenceData Service
                            if (apprenticeship.FrameworkCode != null && apprenticeship.ProgType != null && apprenticeship.PathwayCode != null)
                            {
                                apprenticeship.ApprenticeshipType = ApprenticeshipType.FrameworkCode;
                                adminReport += $"> Framework Apprenticeship - FrameworkCode ( { apprenticeship.FrameworkCode } ), ProgType ( { apprenticeship.ProgType } ), PathwayCode ( { apprenticeship.PathwayCode } )" + Environment.NewLine;
                            }
                            else if (apprenticeship.StandardCode != null && apprenticeship.Version != null)
                            {
                                apprenticeship.ApprenticeshipType = ApprenticeshipType.StandardCode;
                                adminReport += $"> Standard Apprenticeship - StandardCode ( { apprenticeship.StandardCode } ), Version ( { apprenticeship.Version } )" + Environment.NewLine;
                            }
                            else
                            {
                                apprenticeship.ApprenticeshipType = ApprenticeshipType.Undefined;
                                apprenticeship.RecordStatus = RecordStatus.MigrationPending;
                                adminReport += $"> * ATTENTION * Apprenticeship NOT Defined - FrameworkCode ( { apprenticeship.FrameworkCode } ), ProgType ( { apprenticeship.ProgType } ), PathwayCode ( { apprenticeship.PathwayCode } ), StandardCode ( { apprenticeship.StandardCode } ), Version ( { apprenticeship.Version } )" + Environment.NewLine;
                            }
                                

                            // Get ApprenticeshipLocations                          
                            string errorMessageGetApprenticeshipLocations = string.Empty;
                            var apprenticeshipLocations = DataHelper.GetApprenticeshipLocationsByApprenticeshipId(apprenticeship.ApprenticeshipId ?? 0, connectionString, out errorMessageGetApprenticeshipLocations);
                            if (!string.IsNullOrEmpty(errorMessageGetApprenticeshipLocations))
                            {
                                adminReport += $"* ATTENTION * { errorMessageGetApprenticeshipLocations }" + Environment.NewLine;
                            }
                            else
                            {
                                foreach (var apprenticeshipLocation in apprenticeshipLocations)
                                {
                                    apprenticeshipLocation.id = Guid.NewGuid();
                                    apprenticeshipLocation.RecordStatus = RecordStatus.Live;
                                    apprenticeshipLocation.CreatedDate = DateTime.Now;
                                    apprenticeshipLocation.CreatedBy = "DFC – Apprenticeship Migration Tool";
                                    adminReport += "__________________________" + Environment.NewLine;
                                    adminReport += $">>> ApprenticeshipLocation with TribalLocationId ( { apprenticeshipLocation.LocationId } )";

                                    // Get ApprenticeshipLocation DeliveryModes
                                    string errorMessageGetDeliveryModes = string.Empty;
                                    var deliveryModes = DataHelper.GetDeliveryModesByApprenticeshipLocationId(apprenticeshipLocation.ApprenticeshipLocationId, connectionString, out errorMessageGetDeliveryModes);
                                    if (!string.IsNullOrEmpty(errorMessageGetDeliveryModes))
                                    {
                                        adminReport += Environment.NewLine + $"* ATTENTION * { errorMessageGetDeliveryModes }" + Environment.NewLine;
                                    }
                                    else
                                    {
                                        apprenticeshipLocation.DeliveryModes = deliveryModes;
                                        // Mapp DeliveryModes to ApprenticeshipLocationType
                                        // 1 - 100PercentEmployer, 2 - DayRelease, 3 - BlockRelease
                                        if ((deliveryModes.Contains(1) && !deliveryModes.Contains(2) && deliveryModes.Contains(3)) ||
                                           (deliveryModes.Contains(1) && deliveryModes.Contains(2) && !deliveryModes.Contains(3)) ||
                                           (deliveryModes.Contains(1) && deliveryModes.Contains(2) && deliveryModes.Contains(3)))
                                        {
                                            apprenticeshipLocation.ApprenticeshipLocationType = ApprenticeshipLocationType.ClassroomBasedAndEmployerBased;
                                            apprenticeshipLocation.LocationType = LocationType.Venue;
                                            apprenticeshipLocation.Radius = VenueBasedRadius; // Leave it as it is. COUR-419
                                            adminReport += $" - ApprenticeshipLocationType ( { apprenticeshipLocation.ApprenticeshipLocationType } )" + Environment.NewLine;
                                        }
                                        else if ((!deliveryModes.Contains(1) && !deliveryModes.Contains(2) && deliveryModes.Contains(3)) ||
                                                 (!deliveryModes.Contains(1) && deliveryModes.Contains(2) && !deliveryModes.Contains(3)) ||
                                                 (!deliveryModes.Contains(1) && deliveryModes.Contains(2) && deliveryModes.Contains(3)))
                                        {
                                            apprenticeshipLocation.ApprenticeshipLocationType = ApprenticeshipLocationType.ClassroomBased;
                                            apprenticeshipLocation.LocationType = LocationType.Venue;
                                            apprenticeshipLocation.Radius = VenueBasedRadius;
                                            adminReport += $" -  ApprenticeshipLocationType ( { apprenticeshipLocation.ApprenticeshipLocationType } )" + Environment.NewLine;
                                        }
                                        else if (deliveryModes.Contains(1) && !deliveryModes.Contains(2) && !deliveryModes.Contains(3))
                                        {
                                            apprenticeshipLocation.ApprenticeshipLocationType = ApprenticeshipLocationType.EmployerBased;
                                            // apprenticeshipLocation.LocationType = Region or SubRegion depending ... bellow;
                                            adminReport += $" -  ApprenticeshipLocationType ( { apprenticeshipLocation.ApprenticeshipLocationType } )" + Environment.NewLine;
                                        }
                                        else
                                        {
                                            apprenticeshipLocation.ApprenticeshipLocationType = ApprenticeshipLocationType.Undefined;
                                            apprenticeshipLocation.LocationType = LocationType.Undefined;
                                            apprenticeshipLocation.RecordStatus = RecordStatus.MigrationPending;
                                            adminReport += Environment.NewLine + $"*** ATTENTION *** ApprenticeshipLocationType ( { apprenticeshipLocation.ApprenticeshipLocationType } )" + Environment.NewLine;
                                        }

                                        // Get Location by Tribal LocationId
                                        string errorMessageGetTribalLocation = string.Empty;
                                        var location = DataHelper.GetLocationByLocationIdPerProvider(apprenticeshipLocation.LocationId ?? 0, provider.ProviderId ?? 0, connectionString, out errorMessageGetTribalLocation);
                                        if (!string.IsNullOrEmpty(errorMessageGetTribalLocation))
                                        {
                                            apprenticeshipLocation.RecordStatus = RecordStatus.MigrationPending;
                                            adminReport += Environment.NewLine + $"*** ATTENTION *** { errorMessageGetTribalLocation }" + Environment.NewLine;
                                        }
                                        else
                                        {
                                            if (location == null)
                                            {
                                                apprenticeshipLocation.RecordStatus = RecordStatus.MigrationPending;
                                                adminReport += $"We couldn't get location for LocationId ({ apprenticeshipLocation.LocationId }) " + Environment.NewLine;
                                            } 
                                        }


                                        // Venue Locations
                                        if ((apprenticeshipLocation.ApprenticeshipLocationType.Equals(ApprenticeshipLocationType.ClassroomBased)) ||
                                        apprenticeshipLocation.ApprenticeshipLocationType.Equals(ApprenticeshipLocationType.ClassroomBasedAndEmployerBased))
                                        {
                                            #region Venue Locations

                                            GetVenuesByPRNAndNameCriteria venueCriteria = new GetVenuesByPRNAndNameCriteria(apprenticeship.ProviderUKPRN.ToString(), location.LocationName);
                                            var venuesResult = Task.Run(async () => await venueService.GetVenuesByPRNAndNameAsync(venueCriteria)).Result;

                                            if (venuesResult.IsSuccess && venuesResult.Value.Value != null)
                                            {
                                                var venues = venuesResult.Value.Value;

                                                if (venues.Count().Equals(1))
                                                {
                                                    // It is a good case 
                                                    var venue = venues.FirstOrDefault();
                                                    if(venue.LocationId == null || venue.LocationId.Equals(0))
                                                    {
                                                        // We don't have valid LocationId assign by our VenueService API
                                                        apprenticeshipLocation.RecordStatus = RecordStatus.MigrationPending;
                                                        adminReport += $"*** ATTENTION *** We don't have valid LocationId assign by our VenueService API - Location/VenueName ({ location.LocationName }). " + Environment.NewLine;
                                                    }
                                                    else
                                                    {
                                                        apprenticeshipLocation.LocationId = venue.LocationId;
                                                        apprenticeshipLocation.LocationGuidId = new Guid(venue.ID);

                                                  
                                                        if (venue.Status.Equals(VenueStatus.Live))
                                                        {
                                                            // Check Venue.TribalLocationId is equal to location.LocationId
                                                            if (venue.TribalLocationId == location.LocationId)
                                                            {
                                                                // It's good match - we don't update Contacts as we asume that they were updated when venue was initially created from the same location
                                                                adminReport += $"It's a good match " + Environment.NewLine;
                                                            }
                                                            else
                                                            {
                                                                // If it's different we will update IDs above
                                                                if (venue.TribalLocationId != null || venue.TribalLocationId.Equals(0))
                                                                {
                                                                    // No valid TribalLocationId (There is not going to be duplication) => Update Venue 
                                                                    adminReport += $"No valid TribalLocationId (There is not going to be duplication) => Update Venue" + Environment.NewLine;
                                                                }
                                                                else
                                                                {
                                                                    // There is already Venue with different TribalLocationId => What to do? Update or NOT
                                                                    adminReport += $"There is already Venue with different TribalLocationId => What to do? Update or NOT" + Environment.NewLine;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            // Bring it to LIVE and Update????
                                                            adminReport += $"Bring it to LIVE ????" + Environment.NewLine;

                                                            // Update Venue.Status
                                                            venue.Status = VenueStatus.Live;
                                                            // Update Contacts (Telephone, Email, Website) 
                                                            venue.PHONE = location.Telephone;
                                                            venue.EMAIL = location.Email;
                                                            venue.WEBSITE = location.Website;

                                                            // ??? VenueService.Update

                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    // Issue raised many times
                                                    adminReport += $"Multiple Venues for the same name ({ location.LocationName }). " + Environment.NewLine;
                                                    foreach(var venue in venues)
                                                    {
                                                        if(venue.TribalLocationId == location.LocationId)
                                                        {
                                                            // That is our Venue. Use it.

                                                        }
                                                        else
                                                        {
                                                            adminReport += $"Venue not used ( { venue.ID } ). " + Environment.NewLine;
                                                        }
                                                    }
                                                }
                                            }
                                            else if (venuesResult.IsSuccess && venuesResult.Value.Value == null)
                                            {
                                                // There is no such Venue - Add it
                                                var addVenue = new Venue(Guid.NewGuid().ToString(),
                                                                           location.ProviderUKPRN,
                                                                           location.LocationName,
                                                                           location.AddressLine1,
                                                                           location.AddressLine2,
                                                                           null,
                                                                           location.Town,
                                                                           location.County,
                                                                           location.Postcode,
                                                                           location.Latitude,
                                                                           location.Longitude,
                                                                           VenueStatus.Live,
                                                                           "DFC – Apprenticeship Migration Tool",
                                                                           DateTime.Now);
                                                addVenue.PHONE = location.Telephone;
                                                addVenue.EMAIL = location.Email;
                                                addVenue.WEBSITE = location.Website;
                                                addVenue.TribalLocationId = location.LocationId;

                                                adminReport += $"Adds Venue for LocationName ({ location.LocationName })" + Environment.NewLine;

                                                if (GenerateJsonFilesLocally)
                                                {
                                                    var addVenueJson = JsonConvert.SerializeObject(addVenue);
                                                    string jsonFileName = string.Format("{0}-Venue-{1}-{2}.json", DateTime.Now.ToString("yyMMdd-HHmmss"), providerUKPRN, location.LocationName.Replace(" ", string.Empty));
                                                    string AddVenuePath = string.Format(@"{0}\AddVenue", JsonApprenticeshipFilesPath);
                                                    if (!Directory.Exists(AddVenuePath))
                                                        Directory.CreateDirectory(AddVenuePath);
                                                    File.WriteAllText(string.Format(@"{0}\{1}", AddVenuePath, jsonFileName), addVenueJson);
                                                }
                                                else
                                                {
                                                    var addVenueResult = Task.Run(async () => await venueService.AddAsync(addVenue)).Result;
                                                    if (addVenueResult.IsSuccess && addVenueResult.HasValue)
                                                    {
                                                        // All good => It's important to update apprenticeshipLocation.LocationId
                                                        apprenticeshipLocation.LocationId = ((Venue)addVenueResult.Value).LocationId;
                                                        adminReport += $"All good => It's important to update apprenticeshipLocation.LocationId" + Environment.NewLine;
                                                    }
                                                    else
                                                    {
                                                        // Problem with the service Error:  { addVenueResult?.Error }
                                                        apprenticeshipLocation.RecordStatus = RecordStatus.MigrationPending;
                                                        adminReport += $"Problem with the service - AddAsync -  Error:  { addVenueResult?.Error }" + Environment.NewLine;
                                                    }
                                                }
                                            }
                                            else
                                            {

                                                // Problem with the service GetVenuesByPRNAndNameAsync Error:  { venueResult?.Error }
                                                apprenticeshipLocation.RecordStatus = RecordStatus.MigrationPending;
                                                adminReport += $"LocationName ({ location.LocationName })Problem with the service - GetVenuesByPRNAndNameAsync Error:  { venuesResult?.Error }" + Environment.NewLine;
                                            }

                                            #endregion

                                        } // Region or SubRegion Locations
                                        else if (apprenticeshipLocation.ApprenticeshipLocationType.Equals(ApprenticeshipLocationType.EmployerBased))
                                        {
                                            #region Region or SubRegion Locations
                                            var allRegionsWithSubRegions = new SelectRegionModel();

                                            string errorMessageGetRegionSubRegion = string.Empty;
                                            var onspdRegionSubregion = DataHelper.GetRegionSubRegionByPostcode(location.Postcode, connectionString, out errorMessageGetRegionSubRegion);
                                            if (!string.IsNullOrEmpty(errorMessageGetRegionSubRegion))
                                            {
                                                adminReport += $"* ATTENTION * { errorMessageGetRegionSubRegion }" + Environment.NewLine;
                                            }
                                            else
                                            {
                                                if (apprenticeshipLocation.Radius > RegionSubRegionRangeRadius)
                                                {
                                                    // It's Region
                                                    var selectedRegion = allRegionsWithSubRegions.RegionItems.Where(x => x.RegionName == onspdRegionSubregion.Region).SingleOrDefault();
                                                    if (selectedRegion == null)
                                                    {
                                                        // Problem - No selectedRegion match => Undefined 
                                                    }
                                                    else
                                                    {
                                                        apprenticeshipLocation.LocationId = selectedRegion.ApiLocationId;
                                                        apprenticeshipLocation.LocationType = LocationType.Region;
                                                        apprenticeshipLocation.Radius = RegionBasedRadius;
                                                    }
                                                }
                                                else
                                                {
                                                    // It's SubRegion, but if SubRegion does not much get Region
                                                    var selectedSubRegion = allRegionsWithSubRegions.RegionItems.Where(x => x.SubRegion.Any(sr => sr.SubRegionName == onspdRegionSubregion.SubRegion)).SingleOrDefault();
                                                    if (selectedSubRegion == null)
                                                    {
                                                        // Do Region
                                                        var selectedRegion = allRegionsWithSubRegions.RegionItems.Where(x => x.RegionName == onspdRegionSubregion.Region).SingleOrDefault();
                                                        if (selectedRegion == null)
                                                        {
                                                            // Problem - No selectedRegion and NO selectedSubRegion match => Undefined 
                                                            // It will be hard to identify
                                                        }
                                                        else
                                                        {
                                                            apprenticeshipLocation.LocationId = selectedRegion.ApiLocationId;
                                                            apprenticeshipLocation.LocationType = LocationType.Region;
                                                            apprenticeshipLocation.Radius = RegionBasedRadius;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        apprenticeshipLocation.LocationId = selectedSubRegion.SubRegion.Where(x => x.SubRegionName == onspdRegionSubregion.SubRegion).SingleOrDefault().ApiLocationId;
                                                        apprenticeshipLocation.LocationType = LocationType.SubRegion;
                                                        apprenticeshipLocation.Radius = SubRegionBasedRadius;
                                                    }
                                                }
                                            }

                                            #endregion
                                        }
                                        else
                                        {
                                            // Undefined - apprenticeshipLocation.ApprenticeshipLocationType
                                            apprenticeshipLocation.RecordStatus = RecordStatus.MigrationPending;
                                        }

                                        // Replace LocationId (TribalLocationId)

                                        // Get Location per 

                                        // Checks locations / Add Locations

                                        // Override Radius / CutOff distance 10

                                        // RecordStatus
                                    }
                                }

                                apprenticeship.ApprenticeshipLocations = apprenticeshipLocations;
                            }


                            // Validate Apprenticeship and Counts
                            if (apprenticeship.ApprenticeshipType.Equals(ApprenticeshipType.Undefined))
                                apprenticeship.RecordStatus = RecordStatus.MigrationPending;

                            // Add Apprenticeship to CosmosDB
                            if (GenerateJsonFilesLocally)
                            {
                                var apprenticeshipJson = JsonConvert.SerializeObject(apprenticeship);
                                string jsonFileName = string.Format("{0}-Apprenticeship-{1}-{2}-{3}.json", DateTime.Now.ToString("yyMMdd-HHmmss"), providerUKPRN, apprenticeship.ApprenticeshipId, apprenticeship?.ApprenticeshipLocations?.Count());
                                if (!Directory.Exists(JsonApprenticeshipFilesPath))
                                    Directory.CreateDirectory(JsonApprenticeshipFilesPath);
                                File.WriteAllText(string.Format(@"{0}\{1}", JsonApprenticeshipFilesPath, jsonFileName), apprenticeshipJson);
                            }
                            else
                            {
                                // Call ApprenticeshipService to Add a Apprenticeship
                            }
                        }
                    }
                }
            }

            if (GenerateReportFilesLocally)
            {
                var adminReportFileName = string.Format("{0}-AdminReport-{1}.txt", DateTime.Now.ToString("yyMMdd-HHmmss"), CountProviders.ToString());
                string AdminReportsPath = string.Format(@"{0}\AdminReports", JsonApprenticeshipFilesPath);
                if (!Directory.Exists(AdminReportsPath))
                    Directory.CreateDirectory(AdminReportsPath);
                File.WriteAllText(string.Format(@"{0}\{1}", AdminReportsPath, adminReportFileName), adminReport);
            }
            Console.WriteLine("Migration of Apprenticeships completed.");
            string lastLine = Console.ReadLine();
        }

        internal static bool CheckForValidUKPRN(string ukprn)
        {
            string regex = "^[1][0-9]{7}$";
            var validUKPRN = Regex.Match(ukprn, regex, RegexOptions.IgnoreCase);

            return validUKPRN.Success;
        }
    }
}

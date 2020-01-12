using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace WinSearchDBAnalyzer
{
    public partial class InputUTCTimeForm : Form
    {
        private Dictionary<String, String> timeZoneDictnary = new Dictionary<string,string>();
        private String UtcTime;
        public InputUTCTimeForm()
        {
            InitializeComponent();
            inputTImeZone();

            foreach (String utcTime in timeZoneDictnary.Keys)
            {
                comboBoxUTCTime.Items.Add(utcTime);
            }
            comboBoxUTCTime.SelectedItem = "UTC+0";
        }

        public String getUTCTime()
        {
            return UtcTime;
        }
        
        private void inputTImeZone(){
            timeZoneDictnary.Add("UTC-12", "Baker Island\r\nU.S. Minor Outlying Islands\r\nHowland Island");
            timeZoneDictnary.Add("UTC-11", "American Samoa\r\nPago Pago\r\nNiue\r\nAlofi\r\nU.S. Minor Outlying Islands\r\nItascatown pre-WW2");
            timeZoneDictnary.Add("UTC-10", "Cook Islands\r\nAvarua\r\nFrench Polynesia\r\nPapeete\r\nU.S. Minor Outlying Islands\r\nMillersville - pre-WW2 settlement\r\nUnited States\r\nHawaii\r\nHonolulu");
            timeZoneDictnary.Add("UTC-9:30", "Atuona\r\nFrench Polynesia");
            timeZoneDictnary.Add("UTC-9", "French Polynesia\r\nRikitea\r\nUnited States");
            timeZoneDictnary.Add("UTC-8", "Adamstown\r\nAlaska\r\nAnchorage\r\nPitcairn Islands\r\nUnited States");
            timeZoneDictnary.Add("UTC-7", "Arizona\r\nBritish Columbia\r\nCalifornia\r\nCanada\r\nLos Angeles\r\nMexicali\r\nMexico\r\nNevada\r\nOregon\r\nPhoenix\r\nSan Diego\r\nSan Francisco\r\nSan Jose\r\nSurrey\r\nTijuana\r\nUnited States\r\nVancouver\r\nWashington\r\nYukon");
            timeZoneDictnary.Add("UTC-6", "Alberta\r\nBelize\r\nBelmopan\r\nCalgary\r\nCanada\r\nCosta Rica\r\nEdmonton\r\nLimón\r\nNorthwest Territories\r\nNunavut\r\nSan José\r\nSaskatchewan");
            timeZoneDictnary.Add("UTC-5", "Acre\r\nAlabama\r\nArequipa\r\nArkansas\r\nAustin\r\nBarranquilla\r\nBogotá\r\nBrazil\r\nCali\r\nCanada\r\nCartagena\r\nCayman Islands\r\nChicago\r\nChile\r\nCiudad Neza\r\nColombia\r\nDallas\r\nEcatepec de Morelos\r\nEcuador\r\nFort Worth\r\nGeorge Town\r\nGuadalajara\r\nGuayaquil\r\nHaiti\r\nHouston\r\nIllinois\r\nIowa\r\nJamaica\r\nKansas\r\nKansas City\r\nKingston\r\nLeón\r\nLima\r\nLouisiana\r\nManitoba\r\nMedellín\r\nMemphis\r\nMexico City\r\nMexico\r\nMilwaukee\r\nMinneapolis\r\nMinnesota\r\nMississippi\r\nMissouri\r\nMonterrey\r\nNashville\r\nNebraska\r\nNorth Dakota\r\nOklahoma\r\nOklahoma City\r\nOmaha\r\nPanama City\r\nPanama\r\nPeru\r\nPort-au-Prince\r\nPuebla\r\nQuito\r\nRio Branco\r\nSan Antonio\r\nSouth Dakota\r\nSpanish Town\r\nTennessee\r\nTexas\r\nTrujillo\r\nTulsa\r\nUnited States\r\nWinnipeg\r\nWisconsin");
            timeZoneDictnary.Add("UTC-4", "Anguilla\r\nAmazonas\r\nAntigua and Barbuda\r\nAruba\r\nAsunción\r\nAtlanta\r\nBahamas\r\nBaltimore\r\nBarbados\r\nBasseterre\r\nBasse-Terre\r\nBolivia\r\nBonaire\r\nSint Eustatius and Saba\r\nBoston\r\nBrazil\r\nBridgetown\r\nBritish Virgin Islands\r\nCampo Grande\r\nCanada\r\nCaracas\r\nCarolina\r\nCastries\r\nChaguanas\r\nCharlotte\r\nCharlotte Amalie\r\nCincinnati\r\nCiudad del Este\r\nCleveland\r\nCockburn Town\r\nColumbus\r\nConnecticut\r\nCuba\r\nCuraçao\r\nDelaware\r\nDetroit\r\nDominica\r\nDominican Republic\r\nFlorida\r\nFort-de-France\r\nGeorgetown\r\nGeorgia\r\nGrenada\r\nGuadeloupe\r\nGustavia\r\nGuyana\r\nHavana\r\nIndiana\r\nIndianapolis\r\nJacksonville\r\nKentucky\r\nKingstown\r\nKralendijk\r\nLa Paz\r\nLexington-Fayette\r\nMaine\r\nManaus\r\nMaracaibo\r\nMaracay\r\nMarigot\r\nMartinique\r\nMaryland\r\nMassachusetts\r\nMato Grosso\r\nMato Grosso do Sul\r\nMiami\r\nMichigan\r\nMontreal\r\nMontserrat\r\nNassau\r\nNew Hampshire\r\nNew Jersey\r\nNew York\r\nNorth Carolina\r\nOhio\r\nOntario\r\nOranjestad\r\nOttawa\r\nPará\r\nParaguay\r\nPennsylvania\r\nPhiladelphia\r\nPittsburgh\r\nPlymouth\r\nPort of Spain\r\nPuerto Rico\r\nQuebec\r\nRaleigh\r\nRhode Island\r\nRoad Town\r\nRondônia\r\nRoraima\r\nRoseau\r\nSaint Kitts and Nevis\r\nSaint Lucia\r\nSaint Martin\r\nSaint Vincent and the Grenadines\r\nSaint-Barthélemy\r\nSan Juan\r\nSanta Cruz\r\nSantiago de Cuba\r\nSantiago de los Caballeros\r\nSanto Domingo\r\nSouth Carolina\r\nSt. George's\r\nSt. John's\r\nStaten Island\r\nTampa\r\nThe Valley\r\nToronto\r\nTrinidad and Tobago\r\nTurks and Caicos Islands\r\nU.S. Virgin Islands\r\nUnited States\r\nVenezuela\r\nVermont\r\nVirginia\r\nVirginia Beach\r\nWashington\r\nD.C.\r\nWest Virginia\r\nWillemstad");
            timeZoneDictnary.Add("UTC-3", "Alagoas\r\nAmapá\r\nArgentina\r\nBahia\r\nBelém\r\nBelo Horizonte\r\nBermuda\r\nBrasília\r\nBrazil\r\nBuenos Aires\r\nCanada\r\nCayenne\r\nCeará\r\nChile\r\nCórdoba\r\nCuritiba\r\nEspírito Santo\r\nFalkland Islands\r\nFederal District\r\nFortaleza\r\nFrench Guiana\r\nGoiás\r\nGreenland\r\nHalifax\r\nHamilton\r\nMaranhão\r\nMinas Gerais\r\nMontevideo\r\nNew Brunswick\r\nNova Scotia\r\nParaíba\r\nParamaribo\r\nParaná\r\nPernambuco\r\nPiauí\r\nPitugfik\r\nPorto Alegre\r\nPrince Edward Island\r\nPuente Alto\r\nRecife\r\nRio de Janeiro\r\nRio Grande do Norte\r\nRio Grande do Sul\r\nRosario\r\nSaint John\r\nSalto\r\nSalvador\r\nSanta Catarina\r\nSantiago\r\nSão Paulo\r\nSergipe\r\nStanley\r\nSuriname\r\nTocantins\r\nUruguay ");
            timeZoneDictnary.Add("UTC-2:30", "Canada\r\nNewfoundland and Labrador\r\nSt Johns");
            timeZoneDictnary.Add("UTC-2", "Brazil\r\nGreenland\r\nNuuk\r\nSaint Pierre and Miquelon\r\nSaint-Pierre\r\nSouth Georgia and South Sandwich Islands");
            timeZoneDictnary.Add("UTC-1", "Cape Verde,Praia");
            timeZoneDictnary.Add("UTC+0", "Azores\r\nAbidjan\r\nAccra\r\nBafatá\r\nBamako\r\nBanjul\r\nBissau\r\nBo\r\nBobo-Dioulasso\r\nBouaké\r\nBurkina Faso\r\nConakry\r\nDakar\r\nFreetown\r\nGambia\r\nGhana\r\nGreenland\r\nGuinea\r\nGuinea-Bissau\r\nIceland\r\nIttoqqortoormiit\r\nIvory Coast\r\nJamestown\r\nKumasi\r\nLiberia\r\nLomé\r\nMali\r\nMauritania\r\nMonrovia\r\nNouadhibou\r\nNouakchott\r\nNzérékoré\r\nOuagadougou\r\nPonta Delgada\r\nReykjavik\r\nSaint Helena\r\nSão Tomé and Príncipe\r\nSenegal\r\nSierra Leone\r\nSikasso\r\nSokodé\r\nTogo\r\nTouba\r\nYamoussoukro");
            timeZoneDictnary.Add("UTC+1", "Aba\r\nAbomey-Calavi\r\nAbuja\r\nAlgeria\r\nAlgiers\r\nAngola\r\nBangui\r\nBata\r\nBenin City\r\nBenin\r\nBimbo\r\nBirmingham\r\nBoumerdas\r\nBrazzaville\r\nBristol\r\nCameroon\r\nCanary Islands\r\nCasablanca\r\nCentral African Republic\r\nChad\r\nCongo-Brazzaville\r\nCongo-Kinshasa\r\nCork\r\nDouala\r\nDublin\r\nEdinburgh\r\nEquatorial Guinea\r\nFaroe Islands\r\nGabon\r\nGlasgow\r\nGuernsey\r\nIbadan\r\nIreland\r\nIsle of Man\r\nJersey\r\nKaduna\r\nKano\r\nKikwit\r\nKinshasa\r\nLagos\r\nLas Palmas\r\nLeeds\r\nLeicester\r\nLibreville\r\nLisbon\r\nLiverpool\r\nLondon\r\nLuanda\r\nMaiduguri\r\nMalabo\r\nManchester\r\nMorocco\r\nMoundou\r\nNamibia\r\nN'dalatando\r\nN'Djamena\r\nNiamey\r\nNiger\r\nNigeria\r\nOran\r\nPointe-Noire\r\nPort Harcourt\r\nPort-Gentil\r\nPorto\r\nPorto-Novo\r\nPortugal\r\nRabat\r\nRundu\r\nSheffield\r\nTórshavn\r\nTunis\r\nTunisia\r\nUnited Kingdom\r\nWestern Sahara\r\nWindhoek\r\nYaoundé\r\nZaria");
            timeZoneDictnary.Add("UTC+2", "Albania\r\nAarhus\r\nAlexandria\r\nAmsterdam\r\nAndorra\r\nAntwerp\r\nAustria\r\nBarcelona\r\nBelgium\r\nBelgrade\r\nBergen\r\nBerlin\r\nBern\r\nBitola\r\nBlantyre\r\nBosnia and Herzegovina\r\nBotswana\r\nBratislava\r\nBrno\r\nBrussels\r\nBudapest\r\nBujumbura\r\nBulawayo\r\nBurundi\r\nCairo\r\nCape Town\r\nCologne\r\nCongo-Kinshasa\r\nCopenhagen\r\nCroatia\r\nCzechia\r\nDebrecen\r\nDenmark\r\nDurban\r\nDurrës\r\nEgypt\r\nEssen\r\nFrance\r\nFrancistown\r\nFrankfurt\r\nGaborone\r\nGermany\r\nGibraltar\r\nGothenburg\r\nGraz\r\nHamburg\r\nHarare\r\nHungary\r\nItaly\r\nJohannesburg\r\nKaliningrad\r\nKigali\r\nKisangani\r\nKitwe\r\nKošice\r\nKrakow\r\nLesotho\r\nLibya\r\nLiechtenstein\r\nLilongwe\r\nLjubljana\r\nŁódź\r\nLubumbashi\r\nLusaka\r\nLuxembourg\r\nLyon\r\nMacedonia\r\nMadrid\r\nMafeteng\r\nMalawi\r\nMalta\r\nManzini\r\nMaputo\r\nMaribor\r\nMarseille\r\nMaseru\r\nMatola\r\nMbabane\r\nMbuji-Mayi\r\nMilan\r\nMonaco\r\nMontenegro\r\nMozambique\r\nMunich\r\nMuyinga\r\nNaples\r\nNetherlands\r\nNice\r\nNiš\r\nNorway\r\nOslo\r\nPalermo\r\nParis\r\nPodgorica\r\nPoland\r\nPort Said\r\nPrague\r\nPretoria\r\nRome\r\nRussia\r\nRwanda\r\nSan Marino\r\nSarajevo\r\nSerbia\r\nSeville\r\nSkopje\r\nSlovakia\r\nSlovenia\r\nSouth Africa\r\nSoweto\r\nSpain\r\nStockholm\r\nStuttgart\r\nSwaziland\r\nSweden\r\nSwitzerland\r\nThe Hague\r\nTirana\r\nToulouse\r\nTripoli\r\nTurin\r\nValencia\r\nValletta\r\nVatican City\r\nVienna\r\nWarsaw\r\nZagreb\r\nZambia\r\nZaragoza\r\nZimbabwe\r\nZurich");
            timeZoneDictnary.Add("UTC+3", "Adana\r\nAddis Ababa\r\nAl Ahmadi\r\nAl Hudaydah\r\nÅland\r\nAleppo\r\nAmman\r\nAnkara\r\nAntananarivo\r\nAsmara\r\nAthens\r\nBaghdad\r\nBahir Dar\r\nBahrain\r\nBasra\r\nBeirut\r\nBelarus\r\nBucharest\r\nBulgaria\r\nBursa\r\nCheren\r\nChişinău\r\nComoros\r\nCyprus\r\nDamascus\r\nDar es Salaam\r\nDaugavpils\r\nDire Dawa\r\nDjibouti\r\nDjibouti\r\nDnipropetrovsk\r\nDodoma\r\nDoha\r\nDonetsk\r\nEritrea\r\nEspoo\r\nEstonia\r\nEthiopia\r\nFinland\r\nGaza\r\nGaziantep\r\nGomel\r\nGreece\r\nGulu\r\nHaifa\r\nHelsinki\r\nIași\r\nIraq\r\nIsrael\r\nIstanbul\r\nIzmir\r\nJeddah\r\nJerusalem\r\nJordan\r\nJuba\r\nKampala\r\nKassala\r\nKaunas\r\nKazan\r\nKenya\r\nKharkiv\r\nKhartoum\r\nKonya\r\nKuwait City\r\nKuwait\r\nKyiv\r\nLatvia\r\nLebanon\r\nLimassol\r\nLira\r\nLithuania\r\nMadagascar\r\nMalakal\r\nMamoutzou\r\nManama\r\nMariehamn\r\nMayotte\r\nMecca\r\nMek’elē\r\nMinsk\r\nMogadishu\r\nMogilev\r\nMoldova\r\nMombasa\r\nMoroni\r\nMoscow\r\nNairobi\r\nNakuru\r\nNazrēt\r\nNicosia\r\nNizhny Novgorod\r\nOdesa\r\nPalestine\r\nPlovdiv\r\nPort Sudan\r\nQatar\r\nRiga\r\nRiyadh\r\nRomania\r\nRostov-on-Don\r\nRussia\r\nSaint Petersburg\r\nSanaa\r\nSaudi Arabia\r\nSofia\r\nSomalia\r\nSouth Sudan\r\nSudan\r\nSyria\r\nTallinn\r\nTanzania\r\nTartu\r\nThessaloniki\r\nTiraspol\r\nToamasina\r\nTripoli\r\nTurkey\r\nUganda\r\nUkraine\r\nVilnius\r\nYemen\r\nZarqa");
            timeZoneDictnary.Add("UTC+4", "Abu Dhabi\r\nArmenia\r\nAs Sīb al Jadīdah\r\nAzerbaijan\r\nBaku\r\nDubai\r\nGanja\r\nGeorgia\r\nKutaisi\r\nMauritius\r\nMuscat\r\nOman\r\nPort Louis\r\nRéunion\r\nRussia\r\nSaint-Denis\r\nSamara\r\nSeychelles\r\nTbilisi\r\nTolyatti\r\nUnited Arab Emirates\r\nVacoas\r\nVictoria\r\nYerevan");
            timeZoneDictnary.Add("UTC+4:30", "Afghanistan\r\nIran\r\nIsfahan\r\nKabul\r\nKandahar\r\nKaraj\r\nMashhad\r\nMazari Sharif\r\nQom\r\nShiraz\r\nTabriz\r\nTehran");
            timeZoneDictnary.Add("UTC+5", "Aqtöbe\r\nAshkabad\r\nChelyabinsk\r\nDushanbe\r\nFaisalabad\r\nFrench Southern Territories\r\nGujranwala\r\nHyderabad\r\nIslamabad\r\nKarachi\r\nKazakhstan\r\nKhujand\r\nLahore\r\nMaldives\r\nMalé\r\nMultan\r\nNamangan\r\nPakistan\r\nPeshawar\r\nPort-aux-Français\r\nQuetta\r\nRawalpindi\r\nRussia\r\nTajikistan\r\nTashkent\r\nTurkmenabat\r\nTurkmenistan\r\nUzbekistan\r\nYekaterinburg");
            timeZoneDictnary.Add("UTC+5:30", "Ahmedabad\r\nBangalore\r\nChennai\r\nColombo\r\nGalkissa\r\nHyderabad\r\nIndia\r\nKanpur\r\nKolkata\r\nMumbai\r\nNew Delhi\r\nPune\r\nSri Lanka\r\nSurat");
            timeZoneDictnary.Add("UTC+5:45", "Biratnagur\r\nKathmandu\r\nNepal\r\nPokhara");
            timeZoneDictnary.Add("UTC+6", "Almaty\r\nAstana\r\nBangladesh\r\nBhutan\r\nBishkek\r\nBritish Indian Ocean Territory\r\nChittagong\r\nComilla\r\nCox’s Bāzār\r\nDhaka\r\nJessore\r\nKazakhstan\r\nKhulna\r\nKyrgyzstan\r\nNarsingdi\r\nNovosibirsk\r\nOmsk\r\nOsh\r\nRajshahi\r\nRangpur\r\nRussia\r\nThimphu\r\nTongi");
            timeZoneDictnary.Add("UTC+6:30", "Burma  Cocos [Keeling] Islands Naypyidaw Yangon");
            timeZoneDictnary.Add("UTC+7", "Bandung\r\nBangkok\r\nBekasi\r\nBiên Hòa\r\nCambodia\r\nChon Buri\r\nChristmas Island\r\nDa Nang\r\nDepok\r\nHaiphong\r\nHanoi\r\nHoChiMinh City\r\nHuế\r\nIndonesia\r\nJakarta\r\nKrasnoyarsk\r\nLaos\r\nMedan\r\nMueang Nonthaburi\r\nMueang Samut Prakan\r\nNha Trang\r\nNovokuznetsk\r\nPakxe\r\nPalembang\r\nPhnom Penh\r\nRussia\r\nSemarang\r\nSouth Tangerang\r\nSurabaya\r\nTakeo\r\nTangerang\r\nThailand\r\nUdon Thani\r\nVientiane\r\nVietnam ");
            timeZoneDictnary.Add("UTC+8", "Antipolo\r\nAustralia\r\nBacolod City\r\nBandar Seri Begawan\r\nBanjarmasin\r\nBeijing\r\nBrunei\r\nChengdu\r\nChina\r\nChongqing\r\nCity of Balikpapan\r\nCity of Cebu\r\nDadiangas\r\nDavao City\r\nDongguan\r\nGuangzhou\r\nHong Kong\r\nIndonesia\r\nIrkutsk\r\nKaohsiung\r\nKhovd\r\nKlang\r\nKota Bharu\r\nKuala Lumpur\r\nMacau\r\nMakassar\r\nMalaysia\r\nMandurah\r\nManila\r\nMongolia\r\nNanjing\r\nPerth\r\nPhilippines\r\nRussia\r\nShanghai\r\nShenzhen\r\nSingapore\r\nTaipei\r\nTaiwan\r\nTianjin\r\nWestern Australia\r\nWuhan\r\nZamboanga City");
            timeZoneDictnary.Add("UTC+8:30", "Hamhung\r\nNorth Korea\r\nPyongyang");
            timeZoneDictnary.Add("UTC+8:45", "Australia");
            timeZoneDictnary.Add("UTC+9", "Ambon City\r\nBusan\r\nChita\r\nDili\r\nEast Timor\r\nErdenet\r\nIncheon\r\nIndonesia\r\nJapan\r\nJayapura\r\nMongolia\r\nOsaka\r\nPalau\r\nRussia\r\nSeoul\r\nSouth Korea\r\nTokyo\r\nUlan Bator\r\nYakutsk\r\nYokohama");
            timeZoneDictnary.Add("UTC+9:30", "Adelaide\r\nAustralia\r\nDarwin\r\nNorthern Territory\r\nSouth Australia");
            timeZoneDictnary.Add("UTC+10", "Australia\r\nAustralian Capital Territory\r\nCanberra\r\nGuam\r\nHagåtña\r\nKhabarovsk\r\nLae\r\nMicronesia\r\nMoen\r\nNew South Wales\r\nNorthern Mariana Islands\r\nPapua New Guinea\r\nPort Moresby\r\nQueensland\r\nRussia\r\nSydney\r\nTasmania\r\nVictoria\r\nVladivostok");
            timeZoneDictnary.Add("UTC+10:30", "Australia");
            timeZoneDictnary.Add("UTC+11", "Australia\r\nHoniara\r\nKingston\r\nMicronesia\r\nNew Caledonia\r\nNorfolk Island\r\nNoumea\r\nPalikir\r\nPort Vila\r\nRussia\r\nSolomon Islands\r\nVanuatu\r\nYuzhno-Sakhalinsk");
            timeZoneDictnary.Add("UTC+12", "Auckland\r\nFiji\r\nKiribati\r\nMajuro\r\nMarshall Islands\r\nMata-Utu\r\nNauru\r\nNew Zealand\r\nPetropavlovsk-Kamchatsky\r\nRussia\r\nSuva\r\nTarawa\r\nTuvalu\r\nU.S. Minor Outlying Islands\r\nWallis and Futuna\r\nWellington");
            timeZoneDictnary.Add("UTC+12:45", "New Zealand");
            timeZoneDictnary.Add("UTC+13", "Apia\r\nKiribati\r\nNuku'alofa\r\nSamoa\r\nTokelau\r\nTonga");
            timeZoneDictnary.Add("UTC+14", "Kiribati\r\nTabwakea Village");


        }

        private void comboBoxUTCTime_SelectedValueChanged(object sender, EventArgs e)
        {
            textBoxCountry.Text = timeZoneDictnary[comboBoxUTCTime.SelectedItem.ToString()].ToString();
            this.UtcTime = comboBoxUTCTime.SelectedItem.ToString();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.UtcTime = comboBoxUTCTime.SelectedItem.ToString();
            Close();
        }

        private void FormInputUTCTime_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.UtcTime = comboBoxUTCTime.SelectedItem.ToString();
            Close();
        }
    }
}

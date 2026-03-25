-- MySQL dump 10.13  Distrib 8.0.42, for Win64 (x86_64)
--
-- Host: localhost    Database: capstone_db
-- ------------------------------------------------------
-- Server version	8.0.42

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `cache`
--

DROP TABLE IF EXISTS `cache`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cache` (
  `id` int NOT NULL AUTO_INCREMENT,
  `uid` int NOT NULL,
  `oid` int NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cache`
--

LOCK TABLES `cache` WRITE;
/*!40000 ALTER TABLE `cache` DISABLE KEYS */;
/*!40000 ALTER TABLE `cache` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `instance`
--

DROP TABLE IF EXISTS `instance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `instance` (
  `oid` int NOT NULL AUTO_INCREMENT,
  `uid` int NOT NULL,
  `bigClass` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `smallClass` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `abilityType` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `sellState` tinyint(1) NOT NULL,
  `cost` int DEFAULT NULL,
  `expireCount` smallint DEFAULT NULL,
  `stat` int NOT NULL,
  `grade` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`oid`),
  KEY `uid` (`uid`),
  CONSTRAINT `instance_ibfk_1` FOREIGN KEY (`uid`) REFERENCES `user` (`uid`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=48882 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `instance`
--

LOCK TABLES `instance` WRITE;
/*!40000 ALTER TABLE `instance` DISABLE KEYS */;
INSERT INTO `instance` VALUES (1,1,'WriteInst','compass','A',0,123,-1,57,'SuperRare'),(2,1,'WriteInst','compass','A',0,-1,-1,57,'SuperRare'),(3,1,'WriteInst','crayon','A',0,-1,-1,19,'Normal'),(4,1,'WriteInst','eraser','A',1,123,-1,43,'SuperRare'),(5,1,'WriteInst','fountainpen','A',1,123,-1,98,'Unique'),(6,8,'WriteInst','glue','A',0,-1,-1,15,'Normal'),(7,8,'WriteInst','highlighterpen','A',0,-1,-1,35,'Rare'),(8,8,'WriteInst','pencil','A',0,-1,-1,56,'SuperRare'),(23,10,'Kitchen','pan','A',1,123,-1,29,'Rare'),(24,10,'Kitchen','plate','A',0,0,-1,41,'SuperRare'),(25,10,'Kitchen','ptowel','A',0,0,-1,33,'Rare'),(2038,8,'Kitchen','cup','A',0,-1,-1,14,'Normal'),(2039,2,'Kitchen','cup','A',0,-1,-1,12,'Normal'),(2040,8,'Kitchen','cup','A',0,-1,-1,40,'Rare'),(2041,2,'Kitchen','cup','A',1,73564,-1,73,'SuperRare'),(2042,2,'Kitchen','coffeepot','C',0,-1,-1,2,'Normal'),(2693,2,'WriteInst','tape','B',0,-1,-1,36,'Normal'),(7111,2,'Kitchen','cup','A',1,3657,-1,94,'Epic'),(7647,2,'WriteInst','glue','A',1,86,-1,98,'Epic'),(9329,2,'Kitchen','ptowel','B',1,4367,-1,20,'Normal');
/*!40000 ALTER TABLE `instance` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `session`
--

DROP TABLE IF EXISTS `session`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `session` (
  `sid` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `uid` int NOT NULL,
  `expire` datetime NOT NULL,
  PRIMARY KEY (`sid`),
  KEY `uid` (`uid`),
  CONSTRAINT `session_ibfk_1` FOREIGN KEY (`uid`) REFERENCES `user` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `session`
--

LOCK TABLES `session` WRITE;
/*!40000 ALTER TABLE `session` DISABLE KEYS */;
INSERT INTO `session` VALUES ('-1277860186977639191',10,'2025-11-07 21:29:16'),('-5468655679623083651',11,'2025-10-19 23:54:16'),('-550090811192829567',8,'2025-11-07 21:43:59'),('-7734818106106249097',13,'2025-10-20 20:42:13'),('634818087883360451',9,'2025-09-27 17:43:08');
/*!40000 ALTER TABLE `session` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user` (
  `uid` int NOT NULL AUTO_INCREMENT,
  `name` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `email` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `level` int NOT NULL,
  `exp` float NOT NULL,
  `money` int NOT NULL,
  `boxSize` int NOT NULL,
  `password_hash` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `email` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (1,'testuser','test',1,0,4468,10,'test'),(2,'choi','ch0702@gmail.com',1,0,706106,10,'scrypt:32768:8:1$gOPYxa27VZn8lf47$5908a7ad40adaacafe20e5af67a28ed9a63b4677f3147933d370634055d6e72238af60fe59bf9bbf642fccf0094aa83856fed8adb03c8ef160b06fd79b9e001f'),(8,'admin','abc@abc.com11',3,2914,14860983,10,'1q2w3e4r!'),(9,'aaa','aaa',1,0,0,10,'aaa'),(10,'admin2','aaaaaaa',1,0,0,10,'1q2w3e4r!'),(11,'aa','aa',1,0,99999999,10,'aa'),(12,'qwe','asd',1,0,99999999,10,'asd'),(13,'abc','aaaa@aaa.com',1,0,0,10,'abc');
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-11-10  0:35:04

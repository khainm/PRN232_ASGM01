// import React, { useState } from 'react';
// import { Card, Form, Button, Table, Row, Col } from 'react-bootstrap';
// import { DateRangePicker } from 'react-date-range';
// import 'react-date-range/dist/styles.css';
// import 'react-date-range/dist/theme/default.css';
// import { format } from 'date-fns';
// import type { NewsStatisticsDTO, CategoryStatisticsDTO, AccountStatisticsDTO } from '../../types/News';

// interface NewsReportProps {
//     onGenerateReport: (startDate: Date, endDate: Date) => void;
//     reportData: NewsStatisticsDTO | null | undefined;
// }

// export const NewsReport: React.FC<NewsReportProps> = ({ onGenerateReport, reportData }) => {
//     return (
//         <Card>
//             <Card.Body>
//                 <h3>News Report</h3>
//                 <Button onClick={() => onGenerateReport(new Date(), new Date())} className="mb-4">
//                     Generate Report
//                 </Button>
                
//                 {reportData && (
//                     <div>
//                         {/* Total News Card */}
//                         <Card className="mb-4">
//                             <Card.Body>
//                                 <h4>Total News</h4>
//                                 <h2>{reportData.totalNews || 0}</h2>
//                                 <div className="mt-3">
//                                     <p>Active News: {reportData.activeNews || 0}</p>
//                                     <p>Inactive News: {reportData.inactiveNews || 0}</p>
//                                     <p>Featured News: {reportData.featuredNews || 0}</p>
//                                     <p>Total Views: {reportData.totalViews || 0}</p>
//                                 </div>
//                             </Card.Body>
//                         </Card>

//                         {/* Category Statistics */}
//                         {reportData.categoryStatistics && reportData.categoryStatistics.length > 0 && (
//                             <Card className="mb-4">
//                                 <Card.Header>
//                                     <h4>Category Statistics</h4>
//                                 </Card.Header>
//                                 <Card.Body>
//                                     <Table striped bordered hover>
//                                         <thead>
//                                             <tr>
//                                                 <th>Category</th>
//                                                 <th>News Count</th>
//                                                 <th>View Count</th>
//                                             </tr>
//                                         </thead>
//                                         <tbody>
//                                             {reportData.categoryStatistics.map((stat: CategoryStatisticsDTO, index: number) => (
//                                                 <tr key={index}>
//                                                     <td>{stat.categoryName}</td>
//                                                     <td>{stat.newsCount}</td>
//                                                     <td>{stat.viewCount}</td>
//                                                 </tr>
//                                             ))}
//                                         </tbody>
//                                     </Table>
//                                 </Card.Body>
//                             </Card>
//                         )}

//                         {/* Account Statistics */}
//                         {reportData.accountStatistics && reportData.accountStatistics.length > 0 && (
//                             <Card className="mb-4">
//                                 <Card.Header>
//                                     <h4>Account Statistics</h4>
//                                 </Card.Header>
//                                 <Card.Body>
//                                     <Table striped bordered hover>
//                                         <thead>
//                                             <tr>
//                                                 <th>Account</th>
//                                                 <th>News Count</th>
//                                                 <th>View Count</th>
//                                             </tr>
//                                         </thead>
//                                         <tbody>
//                                             {reportData.accountStatistics.map((stat: AccountStatisticsDTO, index: number) => (
//                                                 <tr key={index}>
//                                                     <td>{stat.fullName}</td>
//                                                     <td>{stat.newsCount}</td>
//                                                     <td>{stat.viewCount}</td>
//                                                 </tr>
//                                             ))}
//                                         </tbody>
//                                     </Table>
//                                 </Card.Body>
//                             </Card>
//                         )}
//                     </div>
//                 )}
//             </Card.Body>
//         </Card>
//     );
// }; 